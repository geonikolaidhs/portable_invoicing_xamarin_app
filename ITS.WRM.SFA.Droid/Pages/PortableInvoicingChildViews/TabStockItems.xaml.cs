using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages.PortableInvoicingChildViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabStockItems : ContentPage
    {

        ObservableCollection<ItemStockPresent> ItemList;
        List<ItemStockPresent> ResultList;
        private static bool EventRegistered = false;

        public TabStockItems()
        {
            InitializeComponent();
            ItemList = new ObservableCollection<ItemStockPresent>();
            ResultList = new List<ItemStockPresent>();
            InitiallizeControllers();
        }
        protected override void OnDisappearing()
        {
            UnregisterFromEvents();
            base.OnDisappearing();
        }
        protected override void OnAppearing()
        {
            RegisterOnEvents();
            Task.Run(async () =>
            {
                await Search("", false);
            });
            base.OnAppearing();
        }
        private void UnregisterFromEvents()
        {
            MessagingCenter.Unsubscribe<App, string>(this, EventNames.SCAN_EVENT);
            EventRegistered = false;
        }
        private void RegisterOnEvents()
        {
            if (!DependencyService.Get<IBlueTooth>().Connected())
            {
                DependencyService.Get<IBlueTooth>().Start(App.SFASettings.BlueToothScanner, eBlueToothDevice.SCANNER);
            }
            if (!EventRegistered)
            {
                MessagingCenter.Subscribe<App, string>(this, EventNames.SCAN_EVENT, (sender, arg) =>
                {
                    Task.Run(async () =>
                    {
                        await OnScan(arg);
                    });
                });
                EventRegistered = true;
            }
        }

        protected async Task OnScan(string code)
        {
            OwnerApplicationSettings ownAppSet = App.OwnerApplicationSettings;
            string paddedCode = (ownAppSet.PadBarcodes) ? code.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : code;
            await Search(paddedCode, true);
        }

        private async Task<ObservableCollection<ItemStockPresent>> Search(string filter, bool fromScanner)
        {
            try
            {

                UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                List<ItemStockPresent> tempList = new List<ItemStockPresent>();
                await Task.Run(() =>
                {
                    List<DocumentHeader> openDocs = App.DbLayer.GetAffectStockOpenDocuments();
                    tempList = string.IsNullOrEmpty(filter) ? App.DbLayer.GetAllStockItems() : App.DbLayer.SearchStockItems(filter, fromScanner);
                    tempList.ForEach(x => x.CalculateQuantityOnOpenDocuments(openDocs));
                });
                ResultList.Clear();
                ResultList = tempList.OrderByDescending(x => x.Stock).ToList();
                ItemList = new ObservableCollection<ItemStockPresent>(ResultList);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ItemDataSource.ItemsSource = ItemList;
                    BarcodeInput.Text = "";
                    DescriptionInput.Text = "";
                    UserDialogs.Instance.HideLoading();
                });
            }
            return ItemList;
        }

        protected async void SearchBarcodeByKeyboard(object sender, EventArgs e)
        {
            try
            {
                await OnScan(BarcodeInput.Text);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void SearchDescriptionByKeyboard(object sender, EventArgs e)
        {
            try
            {
                await Search(DescriptionInput.Text, false);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void OnNewUnloadingDocument(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                string userAnswer;
                DocumentHeader header = null;
                DocumentType docType = App.DbLayer.GetDocumentTypeById(App.SFASettings.UnLoadingDocumentTypeOid);
                if (docType == null)
                {
                    DocumentCustomerPage(docType, out userAnswer);
                    if (userAnswer == ResourcesRest.Cancel)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(ResourcesRest.MsgTypeAlert, "User Cancel", ResourcesRest.MsgBtnOk);
                        });
                        return;
                    }
                    if (docType == null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.UnLoadingDocumentType + " " + "Not Found", ResourcesRest.MsgBtnOk);
                        });
                        return;
                    }
                }
                else
                {
                    Address documentAddress = null;
                    Customer documentCustomer = null;
                    string errorMessage = string.Empty;
                    await Task.Run(() =>
                    {
                        GetCustomerAndAddressForDocument(out documentCustomer, out documentAddress, out errorMessage);
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert(ResourcesRest.MsgTypeAlert, errorMessage, ResourcesRest.MsgBtnOk);
                            });
                            return;
                        }
                        documentCustomer = App.DbLayer.GetCustomerByTrader(App.Owner.TraderOid);
                        header = DocumentHelper.GenareteDocumentHeader(App.DbLayer, documentCustomer, docType, documentAddress, App.Store);
                        Dictionary<Item, string> fails = new Dictionary<Item, string>();
                        Dictionary<Item, string> successes = new Dictionary<Item, string>();
                        List<Item> itemList = App.DbLayer.GetStockItems();
                        if (itemList.Count <= 0)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.OutOfStock, ResourcesRest.MsgBtnOk);
                                UserDialogs.Instance.HideLoading();
                            });
                            return;
                        }
                        else
                        {
                            App.DbLayer.UpdateDocumentHeader(header);
                            foreach (Item item in itemList)
                            {
                                try
                                {
                                    DocumentDetail documentDetail = DocumentHelper.CreateNewDocumentDetail(ref header, item, null, item.DefaultBarcodeOid, (decimal)item.Stock, App.DbLayer);
                                    if (documentDetail != null)
                                    {
                                        DocumentHelper.AddItem(ref header, documentDetail, App.DbLayer, false, true);
                                        documentDetail.ItemOid = item.Oid;
                                        documentDetail.DocumentHeaderOid = header.Oid;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    App.LogError(ex);
                                    fails.Add(item, ex.Message);
                                }
                            }
                            App.DbLayer.UpdateDocumentHeader(header);
                            if (fails.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (KeyValuePair<Item, string> fail in fails)
                                {
                                    sb.Append(fail.Key.Code + " (" + fail.Value + "), ");
                                }
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + sb.ToString(), ResourcesRest.MsgBtnOk);
                                });
                            }
                        }
                        if (header.DocumentDetails.Count > 0)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushAsync(new DocumentTabPage(header));
                            });
                            return;
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.OutOfStock, ResourcesRest.MsgBtnOk);
                                UserDialogs.Instance.HideLoading();
                            });
                            return;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
            UserDialogs.Instance.HideLoading();
        }


        private void GetCustomerAndAddressForDocument(out Customer customer, out Address address, out string errorMessage)
        {
            customer = null;
            address = null;
            errorMessage = string.Empty;
            customer = App.DbLayer.GetCustomerByTrader(App.Owner.TraderOid);
            address = App.DbLayer.GetById<Address>(App.Store.AddressOid);
            if (customer == null && address != null)
            {
                Trader trader = App.DbLayer.GetTraderById(address.TraderOid);
                if (trader != null)
                {
                    customer = App.DbLayer.GetCustomerByTrader(trader.Oid);
                }
            }
            if (address == null)
            {
                address = App.DbLayer.GetAddressByTrader(App.Owner.TraderOid).FirstOrDefault();
            }
            if (address == null && customer != null)
            {
                address = customer.GetDefaultAddress(App.DbLayer);
            }
            if (customer == null)
            {
                errorMessage = "Customer Not Found";
            }
            if (address == null)
            {
                errorMessage = errorMessage == string.Empty ? " Address Not Found " : errorMessage = " Customer And Address Not Found ";
            }

        }

        private void DocumentCustomerPage(DocumentType docType, out string userAnswer)
        {
            userAnswer = string.Empty;
            Dictionary<DocumentType, string> DocumentList = new Dictionary<DocumentType, string>();
            List<string> ListDocs = App.DbLayer.GetDocumentList(App.Store.Oid, out DocumentList);
            string doc = "";
            if (ListDocs.Count > 0)
            {
                var action = DisplayActionSheet(ResourcesRest.ChooseDocumentType, ResourcesRest.Cancel, null, ListDocs.ToArray());
                doc = action.ToString();
                docType = DocumentList.Where(x => x.Value == doc).FirstOrDefault().Key;
                if (doc == ResourcesRest.Cancel)
                {
                    userAnswer = ResourcesRest.Cancel;
                    return;
                }
                if (docType != null)
                {

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new DocumentCustomerPage(docType));
                    });
                    UserDialogs.Instance.HideLoading();
                }
            }
            else
            {
                throw new Exception("No Valid DocumentType Found");
            }
        }
        protected async void BtnSearchItem(object sender, EventArgs e)
        {
            try
            {
                OwnerApplicationSettings ownAppSet = App.OwnerApplicationSettings;
                bool fromScanner = false;
                string filter = string.Empty;
                if (!string.IsNullOrEmpty(BarcodeInput.Text))
                {
                    fromScanner = true;
                    filter = (ownAppSet.PadBarcodes) ? BarcodeInput.Text.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : BarcodeInput.Text;
                }
                else
                {
                    filter = DescriptionInput.Text;
                }
                await Search(filter, fromScanner);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            BarcodeInput.Placeholder = ResourcesRest.ProductPageSearchByCodeOrBaercode;
            DescriptionInput.Placeholder = ResourcesRest.SearchByDescription;
            lblItemCode.Text = ResourcesRest.Code;
            lblStock.Text = ResourcesRest.Stock;
            lblItemDescription.Text = ResourcesRest.Description;
            BtnNewDocumet.Text = ResourcesRest.NewUnloadingDocument;
            lblCommitedQuantity.Text = ResourcesRest.QuantityOnOpenDocuments;
        }

        protected void OnDescriptionInputFocused(object sender, EventArgs e)
        {
            DescriptionInput.Text = string.Empty;
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        protected void OnDescriptionInputUnFocused(object sender, EventArgs e)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void OnBarcodeInputFocused(object sender, EventArgs e)
        {
            BarcodeInput.Text = string.Empty;
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        protected void OnBarcodeInputUnFocused(object sender, EventArgs e)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
    }
}
