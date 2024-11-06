using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Droid.Pages.DocumentChildViews;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentItemsPage : ContentPage
    {
        private string strNoResultsFoundMessage;
        ObservableCollection<DocumentDetailPresentation> ObservalDocumentDetails = new ObservableCollection<DocumentDetailPresentation>();
        private List<DocumentDetail> DocumentDetailResults = new List<DocumentDetail>();
        bool IsShowingFirstTime = true;
        DocumentHeader _DocumentHeader = new DocumentHeader();
        private string strMsgTypeAlert;
        private string strMsgOk;
        private string msgIsLinkedItem;
        private string strMsgFailure;
        private static bool EventRegistered = false;
        private string msgNullAddres;
        private string msgAlerttype;
        private string msgBtnOk;


        public DocumentItemsPage(DocumentHeader documentHeader)
        {
            InitializeComponent();
            InitiallizeControllers();
            _DocumentHeader = documentHeader;
            LoadDetails(_DocumentHeader.DocumentDetails);
            IsShowingFirstTime = false;
            srchBarcode.Focused += (sender, e) =>
            {
                srchDescription.Text = string.Empty;
            };
            srchDescription.Focused += (sender, e) =>
            {
                srchBarcode.Text = string.Empty;
            };
        }

        protected override void OnDisappearing()
        {
            UnregisterFromEvents();
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            RegisterOnEvents();
            base.OnAppearing();
            if (!IsShowingFirstTime)
            {
                LoadDetails(_DocumentHeader.DocumentDetails);
            }
        }

        private void UnregisterFromEvents()
        {
            MessagingCenter.Unsubscribe<App, string>(this, EventNames.SCAN_EVENT);
            EventRegistered = false;
        }

        private void RegisterOnEvents()
        {
            if (!EventRegistered)
            {
                if (!DependencyService.Get<IBlueTooth>().Connected())
                {
                    DependencyService.Get<IBlueTooth>().Start(App.SFASettings.BlueToothScanner, eBlueToothDevice.SCANNER);
                }
                if (!EventRegistered)
                {
                    MessagingCenter.Subscribe<App, string>(this, EventNames.SCAN_EVENT, (sender, arg) =>
                    {
                        OnScan(arg);
                    });
                    EventRegistered = true;
                }
                EventRegistered = true;
            }
        }

        protected async void OnScan(string code)
        {
            try
            {
                string filter = code.ToString().TrimEnd('\r');
                OwnerApplicationSettings ownAppSet = App.OwnerApplicationSettings;
                string paddedCode = (ownAppSet.PadBarcodes) ? filter.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : filter;
                await Task.Run(() =>
                {
                    SearchByBarcode(paddedCode);
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }



        public int SearchByBarcode(string srchBarcode)
        {
            if (srchBarcode == null)
            {
                return 0;
            }
            else
            {
                try
                {
                    srchBarcode.ToUpper();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ItemDetailsListView.ItemsSource = ObservalDocumentDetails.Where(item => item.BarcodeCode.Contains(srchBarcode));
                    });
                    return 1;
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                        UserDialogs.Instance.HideLoading();
                    });
                    return 0;
                }
            }
        }

        public void SearchByDescription(string Description)
        {
            try
            {
                if (string.IsNullOrEmpty(Description))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ItemDetailsListView.ItemsSource = ObservalDocumentDetails;
                    });
                }
                else
                {
                    string SearchValue = Description.ToUpper();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ItemDetailsListView.ItemsSource = ObservalDocumentDetails.Where(x => x.Item.Name.ToUpper().Contains(SearchValue) || x.ItemCode.Contains(SearchValue));
                    });
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void btnSearchItem(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(srchBarcode.Text))
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        SearchByBarcode(srchBarcode.Text.ToUpper());
                    });
                    UserDialogs.Instance.HideLoading();
                }
                else
                {
                    string searchText = string.IsNullOrEmpty(srchDescription.Text) ? "" : srchDescription.Text.ToUpper();
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        SearchByDescription(searchText);
                    });
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        async void SearchBarcodeByKey(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(srchBarcode.Text))
                {
                    return;
                }

                else if (!string.IsNullOrEmpty(srchBarcode.Text))
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        SearchByBarcode(srchBarcode.Text.ToUpper());
                    });
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
        async void SearchItemByKey(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(srchDescription.Text))
                {
                    return;
                }
                else if (!string.IsNullOrEmpty(srchDescription.Text))
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        SearchByDescription(srchDescription.Text.ToUpper());
                    });
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        public void OnInsert(object sender, EventArgs eventArguments)
        {
            var masterPage = this.Parent as TabbedPage;
            masterPage.CurrentPage = masterPage.Children[1]; //Go to order item page
            masterPage.CurrentPage.Focus();
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblOrderForm.Text = ResourcesRest.OrderFormPageLblOrderForm;
            srchBarcode.Placeholder = ResourcesRest.OrderFormPageSrchBarcode;
            srchDescription.Placeholder = ResourcesRest.OrderFormPageSrchDescription;
            btnInsert.Text = ResourcesRest.InsertItems;
            strNoResultsFoundMessage = ResourcesRest.ProductPageItemNotFount;
            SearchHeader.Text = ResourcesRest.DocumentDetails;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;
            msgIsLinkedItem = ResourcesRest.msgIsLinkedItem;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            msgNullAddres = ResourcesRest.NullDeliveryAddress;
            msgAlerttype = ResourcesRest.MsgTypeAlert;
            msgBtnOk = ResourcesRest.MsgBtnOk;
        }

        async void EditItem(object sender, EventArgs e)
        {
            try
            {
                if (_DocumentHeader.IsSynchronized)
                {
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                    return;
                }
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                DocumentDetail itemDetails = ObservalDocumentDetails.Where(ItemOid => ItemOid.Oid.Equals(menuItem.CommandParameter)).First();

                if (itemDetails.LinkedLine != Guid.Empty)
                {
                    await DisplayAlert(strMsgTypeAlert, msgIsLinkedItem, strMsgOk);
                    return;
                }
                DocumentDetail selectedDocumentDetail = DocumentDetailResults.Where(x => x.Oid == itemDetails.Oid).FirstOrDefault();
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(DocumentDetailPage))
                {
                    await Navigation.PushModalAsync(new DocumentDetailPage(ref _DocumentHeader, selectedDocumentDetail), true);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        async void DeleteItem(object sender, EventArgs e)
        {
            try
            {
                if (_DocumentHeader.IsSynchronized)
                {
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                    return;
                }
                UserDialogs.Instance.ShowLoading(ResourcesRest.CalculatePleaseWait, MaskType.Black);
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                DocumentDetail itemDetails = ObservalDocumentDetails.Where(ItemOid => ItemOid.Oid.Equals(menuItem.CommandParameter)).First();
                if (itemDetails.LinkedLine != Guid.Empty)
                {
                    await DisplayAlert(strMsgTypeAlert, msgIsLinkedItem, strMsgOk);
                    return;
                }
                DocumentDetail selectedDocumentDetail = DocumentDetailResults.Where(x => x.Oid == itemDetails.Oid).FirstOrDefault();
                if (selectedDocumentDetail != null)
                {
                    await Task.Run(() =>
                    {
                        DocumentHelper.DeleteItem(ref _DocumentHeader, selectedDocumentDetail, App.DbLayer);
                        DocumentHelper.RecalculateDocumentCosts(ref _DocumentHeader, App.DbLayer, true, false);
                        App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
                    });
                }
                LoadDetails(_DocumentHeader.DocumentDetails);
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private async void LoadDetails(List<DocumentDetail> ListDetails)
        {
            try
            {
                DocumentDetailResults.Clear();
                ObservalDocumentDetails.Clear();
                foreach (DocumentDetail detail in ListDetails)
                {
                    detail.DocumentHeader = _DocumentHeader;
                    DocumentDetailResults.Add(detail);
                    if (detail.Item == null && detail.ItemOid != null && detail.ItemOid != Guid.Empty)
                    {
                        detail.Item = App.DbLayer.GetById<Item>(detail.ItemOid);
                    }

                    decimal packQty = detail.Qty;
                    if (detail.PackingQuantity > 0 && detail.Qty != detail.PackingQuantity)
                    {
                        packQty = detail.PackingQuantity;
                    }
                    ObservalDocumentDetails.Add(new DocumentDetailPresentation()
                    {
                        CodeDescr = ResourcesRest.orderCode + ": " + detail.Item?.Code,
                        NameDescr = ResourcesRest.orderDescr + ": " + detail.Item?.Name,
                        TotalVatAmount = detail.TotalVatAmount,
                        Qty = detail.Qty,
                        NetTotal = detail.NetTotal,
                        UnitPrice = detail.PriceListUnitPrice,
                        TotalDiscountAmountWithoutVAT = detail.TotalDiscountAmountWithoutVAT,
                        TotalDiscountAmountWithVAT = detail.TotalDiscountAmountWithVAT,
                        GrossTotal = detail.GrossTotal,
                        TotalVatAmountDescr = ResourcesRest.orderVat + ": " + detail.TotalVatAmount.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")),
                        QtyDescr = ResourcesRest.orderQTY + ":    " + detail.Qty,
                        PackingQtyDescr = ResourcesRest.PackQty + ":    " + packQty.ToString(),
                        UnitPriceDescr = ResourcesRest.PriceWithVat + ": " + detail.PriceListUnitPriceWithVAT.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) + "    ",
                        UnitPriceDescrWithoutvat = ResourcesRest.PriceWithoutVat2 + ": " + detail.PriceListUnitPriceWithoutVAT.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) + "    ",
                        TotalDiscountAmountWithoutVATDescr = ResourcesRest.DiscountWithoutVat + ": " + detail.TotalDiscountAmountWithoutVAT.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")),
                        TotalDiscountAmountWithVATDescr = ResourcesRest.DiscountWithVat + ": " + detail.TotalDiscountAmountWithVAT.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")),
                        GrossTotalDescr = ResourcesRest.orderTotal + ":      " + detail.GrossTotal.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) + "      ",
                        GrossTotalDescrWithoutVat = ResourcesRest.DocumentTotalWitoutVat + ":      " + (detail.GrossTotal - detail.TotalVatAmount).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) + "      ",
                        Oid = detail.Oid,
                        Item = detail.Item,
                        ItemOid = detail.ItemOid,
                        ItemCode = detail.Item.Code,
                        BarcodeCode = detail.BarcodeCode
                    });
                }
                ItemDetailsListView.ItemsSource = ObservalDocumentDetails;
                SearchHeader.Text = string.Format("{0} {1}: ({2}),  {3}: ({4}), {5}: ({6})",
                    ResourcesRest.DocumentDetails,
                    ResourcesRest.ItemsCount,
                    ListDetails.Count(),
                    ResourcesRest.orderTotal + " " + ResourcesRest.WithVat,
                    ListDetails.Sum(x => x.GrossTotal).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")),
                    ResourcesRest.orderTotal + " " + ResourcesRest.WithoutVat,
                    ListDetails.Sum(x => x.NetTotal).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR"))
                    );
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        public void DescriptionFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void DescriptionUnFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void BarcodeFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void BarcodeUnFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
    }
}
