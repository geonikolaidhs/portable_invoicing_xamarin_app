using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Droid.Pages.DocumentChildViews;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentTabPage : TabbedPage
    {

        private string lblSumOrder;
        private string lblInsertItems;
        private string LblItemCategories;
        private string lblOrderForms;
        private string lblDetails;
        private string LblPaymentMethods;
        private string msgNullAddres;
        private string msgAlerttype;
        private string msgBtnOk;
        private static bool EventRegistered = false;
        List<ItemDetail> ItemsToAdd = new List<ItemDetail>();
        DocumentHeader _CurrentDocumentHeader = new DocumentHeader();

        public DocumentTabPage(DocumentHeader documentHeader)
        {
            try
            {
                _CurrentDocumentHeader = documentHeader;
                InitializeComponent();
                InitiallizeControllers();
                NavigationPage.SetHasBackButton(this, true);
                if (_CurrentDocumentHeader.DocumentType == null)
                {
                    _CurrentDocumentHeader.DocumentType = App.DocumentTypes.Where(x => x.Oid == documentHeader.DocumentTypeOid).FirstOrDefault();
                }
                if (_CurrentDocumentHeader.DocumentType.Division == null)
                {
                    _CurrentDocumentHeader.DocumentType.Division = App.DbLayer.GetById<Division>(_CurrentDocumentHeader.DocumentType.DivisionOid);
                }
                AddChildrenViews(_CurrentDocumentHeader, documentHeader.Customer, _CurrentDocumentHeader.DocumentType);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        public DocumentTabPage(Customer selectedCustomer, DocumentType docType, string DeliveryAddress, Address selectedAddress)
        {
            try
            {
                InitializeComponent();
                InitiallizeControllers();
                NavigationPage.SetHasBackButton(this, true);
                _CurrentDocumentHeader = DocumentHelper.GenareteDocumentHeader(App.DbLayer, selectedCustomer, docType, selectedAddress, null);
                if (_CurrentDocumentHeader == null)
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.NoResultsFoundMessage, ResourcesRest.MsgBtnOk);
                    return;
                }
                if (_CurrentDocumentHeader.DocumentType.Division == null)
                {
                    _CurrentDocumentHeader.DocumentType.Division = App.DbLayer.GetById<Division>(_CurrentDocumentHeader.DocumentType.DivisionOid);
                }
                App.DbLayer.InsertNewDocumentHeader(_CurrentDocumentHeader);
                AddChildrenViews(_CurrentDocumentHeader, _CurrentDocumentHeader.Customer, docType);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        private void AddChildrenViews(DocumentHeader documentParam, Customer customerParam, DocumentType docType)
        {
            Children.Add(new DocumentTotalPage(documentParam, customerParam) { Title = lblSumOrder });
            if (docType.Division.Section != eDivision.Financial)
            {
                Children.Add(new DocumentProductCategoriesPage(documentParam) { Title = lblInsertItems });
                Children.Add(new DocumentItemsPage(documentParam) { Title = lblDetails });
            }
            Children.Add(new DocumentSelectedCustomerPage(documentParam) { Title = ResourcesRest.DocumentCustomer });
            if (docType.UsesPaymentMethods)
            {
                Children.Add(new DocumentPayments(documentParam) { Title = ResourcesRest.PaymentMethods });
            }
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblOrderForms = ResourcesRest.NewOrderLblOrderForm;
            lblSumOrder = ResourcesRest.NewOrderLblSumOrder;
            lblInsertItems = ResourcesRest.NewOrderTabInsertItemPage;
            LblItemCategories = ResourcesRest.NewOrderTabItemCategories;
            lblDetails = ResourcesRest.DocumentDetails;
            LblPaymentMethods = ResourcesRest.OrderPaymentMethodsTab;
            msgNullAddres = ResourcesRest.NullDeliveryAddress;
            msgAlerttype = ResourcesRest.MsgTypeAlert;
            msgBtnOk = ResourcesRest.MsgBtnOk;
        }

        private void UpdateItemsToAddList(List<ItemDetail> items)
        {
            ItemsToAdd = items;
        }

        private void UpdateCurrentDocumentHeader(DocumentHeader header)
        {
            _CurrentDocumentHeader = header;
        }

        protected override bool OnBackButtonPressed()
        {
            if (App.SkipAsking == false)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        if (ItemsToAdd.Count() > 0)
                        {
                            var result = await this.DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.ConfirmYouHaveChangesToThisDocument, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                            if (result)
                            {
                                AddItems();
                                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.MsgUpdateDocument, ResourcesRest.MsgBtnOk);
                                await NavigateToDocumentList();
                            }
                            else
                            {
                                await NavigateToDocumentList();
                            }
                        }
                        else
                        {
                            await NavigateToDocumentList();
                        }
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                });
            }
            return false;
        }


        protected override void OnDisappearing()
        {
            try
            {
                UnregisterFromEvents();
                base.OnDisappearing();
                BackButtonManager.Instance.RemoveBackButtonListener();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        protected override void OnAppearing()
        {
            RegisterOnEvents();
            base.OnAppearing();
            bool OnBackButtonPressed()
            {
                if (App.SkipAsking == false)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (ItemsToAdd.Count() > 0)
                        {
                            var result = await this.DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.ConfirmYouHaveChangesToThisDocument, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                            if (result)
                            {
                                AddItems();
                                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.MsgUpdateDocument, ResourcesRest.MsgBtnOk);
                                await NavigateToDocumentList();
                            }
                            else
                            {
                                await NavigateToDocumentList();
                            }
                        }
                        else
                        {
                            await NavigateToDocumentList();
                        }
                    });
                }
                return false;
            }
            BackButtonManager.OnBackButtonPressedDelegate onBackButtonPressedDelegate = new BackButtonManager.OnBackButtonPressedDelegate(OnBackButtonPressed);
            BackButtonManager.Instance.SetBackButtonListener(onBackButtonPressedDelegate);
        }



        private async Task NavigateToDocumentList()
        {
            try
            {
                //await Task.Delay(1);
                if (_CurrentDocumentHeader != null && _CurrentDocumentHeader.Oid != Guid.Empty)
                {
                    App.DbLayer.DeleteEmptyDocument(_CurrentDocumentHeader.Oid);
                }
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                int removeINdex = stack.Count - 1;
                if (removeINdex > 0)
                {
                    if (stack[removeINdex].GetType() == typeof(DocumentTabPage))
                    {
                        Navigation.RemovePage(stack[removeINdex]);
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        private void UnregisterFromEvents()
        {
            MessagingCenter.Unsubscribe<App, List<ItemDetail>>(this, EventNames.UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT);
            MessagingCenter.Unsubscribe<App, DocumentHeader>(this, EventNames.UPDATE_CURRENT_DOCUMENT_EVENT);
            EventRegistered = false;
        }

        private void RegisterOnEvents()
        {
            if (!EventRegistered)
            {
                MessagingCenter.Subscribe<App, List<ItemDetail>>(this, EventNames.UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT, (sender, arg) =>
                {
                    UpdateItemsToAddList(arg);
                });
                MessagingCenter.Subscribe<App, DocumentHeader>(this, EventNames.UPDATE_CURRENT_DOCUMENT_EVENT, (sender, arg) =>
                {
                    UpdateCurrentDocumentHeader(arg);
                });
                EventRegistered = true;
            }
        }



        async void AddItems()
        {
            List<string> fails = new List<string>();
            try
            {
                int curentLine = 0;
                int count = ItemsToAdd.Count();
                DocumentHeader referenced = _CurrentDocumentHeader;
                foreach (ItemDetail currentItem in ItemsToAdd)
                {
                    curentLine++;
                    try
                    {
                        CreateDocumentDetail(currentItem, ref referenced);
                    }
                    catch (Exception ex)
                    {
                        fails.Add(currentItem.ItemOid.ToString());
                        continue;
                    }
                }
                DocumentHelper.RecalculateDocumentCosts(ref referenced, App.DbLayer, false, false);
                App.DbLayer.UpdateDocumentHeader(_CurrentDocumentHeader);
                if (fails.Count > 0)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg, ResourcesRest.MsgBtnOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                ItemsToAdd = new List<ItemDetail>();
                UserDialogs.Instance.HideLoading();
            }
        }

        void CreateDocumentDetail(ItemDetail item, ref DocumentHeader referenced)
        {
            if (item.Item == null)
            {
                item.Item = App.DbLayer.GetItem(item.ItemOid);
            }
            if (item.Barcode == null)
            {
                item.Barcode = App.DbLayer.GetBarcodeById(item.BarcodeOid);
            }
            List<LinkedItem> LinkedItems = App.DbLayer.GetLinkedItemsCustom(item.ItemOid);
            foreach (var CurrentItem in LinkedItems)
            {
                CurrentItem.Item = item.Item;
                CurrentItem.ItemOid = item.Item.Oid;
            }
            item.Item.LinkedItems = LinkedItems;
            item.Item.DefaultBarcode = item.Barcode;
            item.Item.DefaultBarcodeOid = item.Barcode.Oid;
            List<DocumentDetailDiscount> DiscountList = new List<DocumentDetailDiscount>();
            DocumentDetail documentDetail = DocumentHelper.ComputeDocumentLine(ref referenced, App.DbLayer, item.Item, item.Barcode, item.TotalQty, false, -1, false, item.Item.Name, DiscountList);
            DocumentHelper.AddItem(ref referenced, documentDetail, App.DbLayer, referenced.DocumentType.ManualLinkedLineInsertion, true);
        }
    }
}
