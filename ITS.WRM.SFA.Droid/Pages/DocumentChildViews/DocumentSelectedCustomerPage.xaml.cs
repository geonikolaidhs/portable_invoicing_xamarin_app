using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
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

namespace ITS.WRM.SFA.Droid.Pages.DocumentChildViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentSelectedCustomerPage : ContentPage
    {

        private string strMsgFailure;
        private string strMsgTypeAlert;
        private string strMsgOk;
        ObservableCollection<CustomerPresent> ObservalCustomers = new ObservableCollection<CustomerPresent>();
        public List<Address> ListAddress;

        private DocumentHeader _DocumentHeader { get; set; }

        public DocumentSelectedCustomerPage(DocumentHeader documentHeader)
        {
            InitializeComponent();
            InitiallizeControllers();
            this.BindingContext = this;
            this._DocumentHeader = documentHeader;

            pckCustomerAddress.SelectedIndexChanged += (sender, args) =>
            {
                if (_DocumentHeader.IsSynchronized)
                {
                    DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                    return;
                }

                if (pckCustomerAddress.SelectedIndex == -1)
                {
                    //AddressOid = Guid.Empty;
                }
                else
                {
                    string AddressStreet = pckCustomerAddress.Items[pckCustomerAddress.SelectedIndex];
                    Address newAddress = ListAddress.Find(x => x.Street.Equals(AddressStreet));
                    UpdateDocumentAddress(newAddress);
                }
            };
            FillData();
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
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            string dots = "      : ";
            SearchHeader.Text = ResourcesRest.Results;
            lblHeader.Text = ResourcesRest.DocumentCustomer;
            lblCustomerName.Text = ResourcesRest.CompName + dots;
            lblCustomerProffession.Text = ResourcesRest.profession + dots;
            lblCustomerCode.Text = ResourcesRest.Customercode + dots;
            lblCustomerTaxCode.Text = ResourcesRest.TaxCode + dots;
            lblCustomerAddress.Text = ResourcesRest.Address + dots;
            lblCustomerPhone.Text = ResourcesRest.phone + dots;
            filter.Placeholder = ResourcesRest.SearchByNameTaxCodeCodePhone;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;
        }

        private void FillData()
        {
            try
            {
                Customer customer = _DocumentHeader.Customer;
                if (customer == null)
                {
                    customer = App.DbLayer.GetCustomer(_DocumentHeader.CustomerOid);
                }
                CustomerName.Text = customer.CompanyName;
                CustomerProffession.Text = customer.Profession;
                CustomerCode.Text = customer.Code;
                Trader CustomerTrader = App.DbLayer.GetTraderById(customer.TraderOid);

                if (CustomerTrader != null)
                {
                    CustomerTaxCode.Text = CustomerTrader.TaxCode;
                }
                if (string.IsNullOrEmpty(customer.CustomerDefaultPhone))
                {
                    Address addr = _DocumentHeader?.BillingAddress;
                    if (addr == null)
                    {
                        addr = App.DbLayer.GetAddressById(_DocumentHeader.BillingAddressOid);
                    }
                    if (addr != null && addr.DefaultPhoneOid != null && addr.DefaultPhoneOid != Guid.Empty)
                    {
                        Phone phone = App.DbLayer.GetPhoneById(addr.DefaultPhoneOid);
                        if (phone != null)
                        {
                            CustomerPhone.Text = phone.Number;
                        }
                    }
                }
                else
                {
                    CustomerPhone.Text = customer.CustomerDefaultPhone;
                }
                LoadAddresses(customer);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        private void LoadAddresses(Customer selectedCustomer)
        {
            ListAddress = App.DbLayer.GetAddressByTrader(selectedCustomer.TraderOid);
            Guid? DefaultAddress = selectedCustomer.DefaultAddressOid;
            pckCustomerAddress.Items.Clear();
            int index = 0;
            foreach (Address address in ListAddress)
            {
                if (address.Street != null)
                {
                    if (!pckCustomerAddress.Items.Contains(address.Street))
                    {
                        pckCustomerAddress.Items.Add(address.Street);
                    }
                }
            }
            foreach (Address address in ListAddress)
            {
                if (_DocumentHeader.BillingAddressOid == address.Oid)
                {
                    pckCustomerAddress.SelectedIndex = index;
                    UpdateDocumentAddress(address);
                    break;
                }
                else if (DefaultAddress.Equals(address.Oid))
                {
                    pckCustomerAddress.SelectedIndex = index;
                    UpdateDocumentAddress(address);
                }
                index++;
            }
        }

        private void UpdateDocumentAddress(Address addr)
        {
            _DocumentHeader.BillingAddress = addr;
            _DocumentHeader.BillingAddressOid = addr?.Oid ?? Guid.Empty;
            _DocumentHeader.AddressProfession = addr?.Profession ?? "";
            App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
        }

        async Task<Customer> RequestChangeCustomer(Guid customerOid, Ref<DocumentHeader> header)
        {
            DocumentHeader documentHeader = header.Value as DocumentHeader;
            Customer NewCustomer = null;
            await Task.Run(() =>
            {
                if (customerOid == Guid.Empty)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, "Customer Not Found", ResourcesRest.MsgBtnOk);
                    });
                    UserDialogs.Instance.HideLoading();
                }

                NewCustomer = App.DbLayer.GetCustomer(customerOid);
                if (NewCustomer == null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, "Customer Not Found", ResourcesRest.MsgBtnOk);
                    });
                    UserDialogs.Instance.HideLoading();
                }
                Trader NewTrader = App.DbLayer.GetTraderById(NewCustomer.TraderOid);
                Store store = documentHeader.Store ?? App.Store;
                Guid oldPriceCatalogPolicy = documentHeader.PriceCatalogPolicyOid;
                PriceCatalogPolicy newPriceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(App.DbLayer, store, NewCustomer);
                Address addr = App.DbLayer.GetAddressById(NewCustomer.DefaultAddressOid);
                if (addr == null)
                {
                    addr = App.DbLayer.GetAddressByTrader(NewCustomer.TraderOid).FirstOrDefault();
                }
                documentHeader.CustomerOid = NewCustomer.Oid;
                documentHeader.Customer = NewCustomer;
                documentHeader.CustomerCode = NewCustomer.Code;
                documentHeader.CustomerName = NewCustomer.CompanyName;
                documentHeader.CustomerLookUpTaxCode = NewTrader.TaxCode;
                documentHeader.PriceCatalogPolicyOid = newPriceCatalogPolicy.Oid;
                documentHeader.PriceCatalogPolicy = newPriceCatalogPolicy;
                documentHeader.UpdatedBy = App.UserId;
                documentHeader.UpdatedOnTicks = DateTime.Now.Ticks;
                documentHeader.BillingAddress = addr;
                documentHeader.BillingAddressOid = addr?.Oid ?? Guid.Empty;
                documentHeader.AddressProfession = addr?.Profession ?? "";
                documentHeader.DeliveryAddress = addr?.City + " " + addr?.Street + " " + addr?.PostCode + " " + addr?.PostCode;


                if (oldPriceCatalogPolicy != newPriceCatalogPolicy.Oid)
                {
                    documentHeader.EffectivePriceCatalogPolicy(newPriceCatalogPolicy);
                }
                DocumentHelper.RecalculateDocumentCosts(ref documentHeader, App.DbLayer, false, false);
                App.DbLayer.UpdateDocumentHeader(documentHeader);
            });
            return NewCustomer;
        }

        async void OnChangeCustomer(object sender, EventArgs e)
        {
            if (_DocumentHeader.IsSynchronized)
            {
                await DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                return;
            }

            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);

            try
            {
                MenuItem item = (MenuItem)sender;
                Guid customerOid = Guid.Empty;
                if (Guid.TryParse(item.CommandParameter.ToString(), out customerOid))
                {
                    await RequestChangeCustomer(customerOid, new Ref<DocumentHeader>(_DocumentHeader));
                }
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, ResourcesRest.MsgBtnOk);
                return;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();

            }

            FillData();
            UserDialogs.Instance.HideLoading();
        }
        void LoadCustomers(string searchFilter)
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
                    CountResults.Text = ResourcesRest.SearchResults + " " + countRes;
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