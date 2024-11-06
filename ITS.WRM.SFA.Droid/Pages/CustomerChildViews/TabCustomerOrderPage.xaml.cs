using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabCustomerOrderPage : ContentPage
    {
        ObservableCollection<OrderPresent> ObservableOrdersPresent = new ObservableCollection<OrderPresent>();
        private Customer CurrentCustomer { get; set; }
        private bool IsShowingFirstTime = true;
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;

        public TabCustomerOrderPage(Customer customer)
        {
            InitializeComponent();
            InitiallizeControllers();
            lblDetail.Text = customer.CompanyName;
            CurrentCustomer = customer;
            IsShowingFirstTime = true;
            Task.Run(async () =>
            {
                await LoadCustomerOrders(customer);
            });
        }
        private void InitiallizeControllers()
        {
            string LanguageId = App.SFASettings.Language;
            if (LanguageId == null)
            {
                LanguageId = "en-GB";
            }
            CultureInfo ci = null;
            ci = DependencyService.Get<ICrossPlatformMethods>().Languageinfo(LanguageId);
            ResourcesRest.Culture = ci;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;
            OrderListItems.Text = ResourcesRest.OrderList;
            CompanyName.Text = ResourcesRest.CompName;
            CustomerCode.Text = ResourcesRest.Customercode;
            CreatedDate.Text = ResourcesRest.CreatedDate;
            FinalizedDate.Text = ResourcesRest.FinalizedDate;
            Status.Text = ResourcesRest.DocumentStatus;
            Type.Text = ResourcesRest.Type;
            NetTotal.Text = ResourcesRest.NetTotal;
            GrossTotal.Text = ResourcesRest.GrossTotal;
            Discount.Text = ResourcesRest.Discount;
            IsSynchronized.Text = ResourcesRest.IsSynchronized;
        }


        private async Task LoadCustomerOrders(Customer customer)
        {
            try
            {
                List<OrderPresent> ListOrders = App.DbLayer.GetCustomerDocuments(customer.Oid);
                if (ListOrders.Count <= 0)
                {
                    ObservableOrdersPresent.Add(new OrderPresent() { CompanyName = ResourcesRest.NoResultsFoundMessage });
                }
                else
                {
                    ObservableOrdersPresent = new ObservableCollection<OrderPresent>(ListOrders);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    OrderList.ItemsSource = ObservableOrdersPresent.OrderByDescending(x => x.FinalizedDate);
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                });
            }
        }

        protected async void EditDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                IsShowingFirstTime = false;
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    Guid HeaderOid = ObservableOrdersPresent.Where(headerOid => headerOid.Oid.Equals(menuItem.CommandParameter)).First().Oid;
                    DocumentHeader selectedHeader = App.DbLayer.LoadDocumentFromDatabase(HeaderOid);
                    List<StorePriceCatalogPolicy> storePolicies = App.DbLayer.GetStorePriceCatalogPolicies(selectedHeader.StoreOid);
                    foreach (StorePriceCatalogPolicy catalog in storePolicies)
                    {
                        catalog.PriceCatalogPolicy = selectedHeader.PriceCatalogPolicy;
                    }
                    IReadOnlyList<Page> stack = Navigation.NavigationStack;
                    if (stack[stack.Count - 1].GetType() != typeof(DocumentTabPage))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushAsync(new DocumentTabPage(selectedHeader), true);
                        });
                    }
                    UserDialogs.Instance.HideLoading();
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                });
                UserDialogs.Instance.HideLoading();
            }

        }

        protected async void DeleteDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }

                var answer = await DisplayAlert(strMsgTypeAlert, ResourcesRest.msgConfirmDelete, ResourcesRest.MsgYes, ResourcesRest.MsgNo);

                if (answer == true)
                {
                    Guid HeaderOid = (Guid)menuItem.CommandParameter;
                    DocumentHeader currentDocument = App.DbLayer.LoadDocumentFromDatabase(HeaderOid);
                    App.DbLayer.RemoveDocumentHeaders(currentDocument);
                    await LoadCustomerOrders(CurrentCustomer);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowingFirstTime)
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                Task.Run(async () =>
                {
                    await LoadCustomerOrders(CurrentCustomer);
                });
                UserDialogs.Instance.HideLoading();
            }
        }
    }
}