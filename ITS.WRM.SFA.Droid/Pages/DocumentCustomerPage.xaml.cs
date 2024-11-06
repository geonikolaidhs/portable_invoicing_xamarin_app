using Acr.UserDialogs;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
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
    public partial class DocumentCustomerPage : ContentPage
    {
        ObservableCollection<CustomerPresent> ObservalCustomers = new ObservableCollection<CustomerPresent>();
        private string strNoResultsFoundMessage;
        private string strMsgTypeAlert;
        private string strMsgOk;
        private string strMsgFailure;
        private DocumentType DocumentType = null;

        public DocumentCustomerPage(DocumentType DocType)
        {
            InitializeComponent();
            InitiallizeControllers();
            Title = "user: " + App.UserName;
            DocumentType = DocType;
        }

        public async void CustomerSearch(object sender, EventArgs e)
        {
            try
            {
                if (srchCustomer.Text == null || srchCustomer.Text == string.Empty || srchCustomer.Text == " " || srchCustomer.Text == "" || srchCustomer.Text.Count() < 2)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.InsertAtLeastTwoCharacters, ResourcesRest.MsgBtnOk);
                    return;
                }
                else
                {
                    string searchText = srchCustomer.Text;
                    searchText.ToUpper();
                    UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        LoadCustomers(searchText);
                    });
                    UserDialogs.Instance.HideLoading();
                }
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

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblCustomer.Text = ResourcesRest.DocumentCustomerPageLblCustomer;
            strNoResultsFoundMessage = ResourcesRest.NoResultsFoundMessage;
            srchCustomer.Placeholder = ResourcesRest.DocumentCustomersrchPlaceHolder;
            SearchHeader.Text = ResourcesRest.SearchResults;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
        }

        async void CreateDocument(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                Guid customerOid = ObservalCustomers.Where(sCode => sCode.Oid.Equals(menuItem.CommandParameter)).FirstOrDefault().Oid;
                Customer selectedCustomer = App.DbLayer.GetCustomer(customerOid);
                Dictionary<Address, string> TraderAddresses = new Dictionary<Address, string>();
                List<string> ListAdress = App.DbLayer.GetCustomerListAdresses(selectedCustomer.DefaultAddressOid, selectedCustomer.TraderOid, out TraderAddresses);
                string Address = "";
                Address selectedAddress = null;
                if (ListAdress.Count > 0)
                {
                    if (ListAdress.Count == 1)
                    {
                        selectedAddress = TraderAddresses.FirstOrDefault().Key;
                    }
                    else
                    {
                        var action = await DisplayActionSheet(ResourcesRest.ChooseDeliveryAddress, ResourcesRest.Cancel, null, ListAdress.ToArray());
                        Address = action;
                        selectedAddress = TraderAddresses.Where(x => x.Value == Address).FirstOrDefault().Key;
                        if (action == ResourcesRest.Cancel)
                        {
                            return;
                        }
                    }
                }
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new DocumentTabPage(selectedCustomer, DocumentType, Address, selectedAddress));
                    });
                    UserDialogs.Instance.HideLoading();
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

        async void SearchCustomerByKeyboard(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    LoadCustomers(srchCustomer.Text);
                });
                UserDialogs.Instance.HideLoading();
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


        protected void LoadCustomers(string searchFilter)
        {
            try
            {
                ObservalCustomers.Clear();
                int countRes = 0;
                List<CustomerPresent> CustomerResults = App.DbLayer.GetCustomerSearch(searchFilter, out countRes);
                ObservalCustomers = CustomerResults == null || countRes == 0 ? new ObservableCollection<CustomerPresent>() { new CustomerPresent() { CompanyName = ResourcesRest.NoResultsFoundMessage } } :
                                                                                                                                                new ObservableCollection<CustomerPresent>(CustomerResults);
                Device.BeginInvokeOnMainThread(() =>
                {
                    CustomerList.ItemsSource = ObservalCustomers;
                    SearchHeader.Text = ResourcesRest.SearchResults + " " + countRes;
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




        public void SearchBarFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void SearchBarUnFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }



    }
}