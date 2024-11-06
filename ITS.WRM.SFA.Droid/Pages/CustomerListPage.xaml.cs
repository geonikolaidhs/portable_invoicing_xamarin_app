using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerListPage : ContentPage
    {
        private string strMsgFailure;
        private string strMsgTypeAlert;
        private string strMsgOk;
        ObservableCollection<CustomerPresent> ObservalCustomers = new ObservableCollection<CustomerPresent>();

        public CustomerListPage()
        {
            InitializeComponent();
            InitiallizeControllers();
            Title = "user: " + App.UserName;
            this.BindingContext = this;
        }

        async void OnMore(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            Guid customerOid = ObservalCustomers.Where(xx => xx.Oid.Equals(item.CommandParameter)).First().Oid;
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            await Task.Run(() =>
            {
                Customer CurrentCustomer = App.DbLayer.GetCustomer(customerOid);
                CurrentCustomer.Trader = App.DbLayer.GetById<Trader>(CurrentCustomer.TraderOid);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PushAsync(new CustomerTabDetailPage(CurrentCustomer));
                });
            });
            UserDialogs.Instance.HideLoading();
        }

        async void OnEdit(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            try
            {
                if (sender == null)
                {
                    return;
                }
                MenuItem item = (MenuItem)sender;
                if (item == null)
                {
                    return;
                }
                Guid customerOid = ObservalCustomers.Where(x => x.Oid.Equals(item.CommandParameter)).First().Oid;
                if (customerOid == Guid.Empty)
                {
                    return;
                }
                await Task.Run(() =>
                {
                    Customer CurrentCustomer = App.DbLayer.GetCustomer(customerOid);
                    CurrentCustomer.Trader = App.DbLayer.GetById<Trader>(CurrentCustomer.TraderOid);
                    if (CurrentCustomer != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushAsync(new CustomerEditPage(CurrentCustomer, true));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }

        }

        async void OnAdd(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            await Task.Run(() =>
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(AddCustomerPage))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(new AddCustomerPage());
                    });
                }
            });
            UserDialogs.Instance.HideLoading();
        }

        public async void CustomerSearch(object sender, EventArgs e)
        {
            try
            {
                if (filter.Text == null || filter.Text == string.Empty || filter.Text == " " || filter.Text == "" || filter.Text.Count() < 2)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.InsertAtLeastTwoCharacters, ResourcesRest.MsgBtnOk);
                    return;
                }
                else
                {
                    string searchText = filter.Text;
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
            SearchHeader.Text = ResourcesRest.SearchResults;
            lblHeader.Text = ResourcesRest.CustomerListLblHeader;
            filter.Placeholder = ResourcesRest.SearchByNameTaxCodeCodePhone;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;
        }
        async void OnNewOrder(object sender, EventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)sender;
                Guid customerOid = ObservalCustomers.Where(sCode => sCode.Oid.Equals(item.CommandParameter)).First().Oid;
                Customer selectedCustomer = App.DbLayer.GetCustomer(customerOid);
                Dictionary<Address, string> TraderAddresses = new Dictionary<Address, string>();
                List<string> ListAdress = App.DbLayer.GetCustomerListAdresses(selectedCustomer.DefaultAddressOid, selectedCustomer.TraderOid, out TraderAddresses);
                string Address = "";
                DocumentType docType = App.DbLayer.GetDocumentTypeById(App.SFASettings.DefaultDocumentTypeOid);
                Address selectedAddress = null;
                if (ListAdress.Count > 0)
                {
                    var action = await DisplayActionSheet(ResourcesRest.ChooseDeliveryAddress, ResourcesRest.Cancel, null, ListAdress.ToArray());
                    Address = action;
                    if (action == ResourcesRest.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    Address = "";
                }

                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    IReadOnlyList<Page> stack = Navigation.NavigationStack;
                    if (stack[stack.Count - 1].GetType() != typeof(DocumentTabPage))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushAsync(new DocumentTabPage(selectedCustomer, docType, Address, selectedAddress));
                        });
                    }
                });
                UserDialogs.Instance.HideLoading();
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

        protected async void SearchCustomerByKeyboard(object sender, EventArgs e)
        {
            try
            {
                ObservalCustomers.Clear();
                if (filter.Text != null && filter.Text != "")
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        LoadCustomers(filter.Text);
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
                    UserDialogs.Instance.HideLoading();
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