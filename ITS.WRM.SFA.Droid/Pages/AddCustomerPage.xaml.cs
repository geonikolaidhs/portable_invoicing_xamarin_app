using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCustomerPage : ContentPage
    {
        private ObservableCollection<TaxOffice> ObservableTaxOffice = new ObservableCollection<TaxOffice>();
        private List<VatLevel> ListVatLevel = new List<VatLevel>();
        private Customer _CurrentCustomer = null;


        public AddCustomerPage()
        {
            InitializeComponent();
            _CurrentCustomer = new Customer();
            _CurrentCustomer.Trader = new Trader();
            _CurrentCustomer.DefaultAddress = new Address();
            _CurrentCustomer.DefaultAddress.IsDefault = true;
            BindingContext = this;
            Stack.IsVisible = true;
            ListVatLevel = App.VatLevels;
            TaxOfficeStack.IsVisible = false;
            InitiallizeControllers();
            InitialiseSelectOptions();

        }

        public async void CheckCustomerOnServer(object sender, EventArgs eventArguments)
        {
            btnShowAddressForm.Opacity = 1;
            btnSaveCustomer.Opacity = 1;
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.MsgSendingData, MaskType.Black);
                if (!DependencyService.Get<ICrossPlatformMethods>().IsConnected())
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.msgNotConnectionFound, ResourcesRest.MsgBtnOk);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txtTaxCode.Text))
                {
                    bool exists = await TaxCodeExistsOnServer(txtTaxCode.Text);
                    if (exists)
                    {
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.TaxCodeExistsOnServer, ResourcesRest.MsgBtnOk);
                        return;
                    }
                    else
                    {
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.AddCustomerMsgNoStoredCustomersMatch, ResourcesRest.MsgBtnOk);
                        return;
                    }
                }
                else
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.AddCustomerMsgNoTaxCode, ResourcesRest.MsgBtnOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.FailContactServer, ResourcesRest.MsgBtnOk);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task<bool> TaxCodeExistsOnServer(string TaxCode)
        {
            return (await ApiHelper.GetTraderByTaxCode(TaxCode)) == null ? false : true;
        }

        public async void SaveCustomer(object sender, EventArgs eventArguments)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.MsgSendingData, MaskType.Black);
            await CreateCustomer();
            UserDialogs.Instance.HideLoading();
        }

        private async Task CreateCustomer()
        {
            string resultmessage = string.Empty;
            try
            {
                bool exists = await TaxCodeExistsOnServer(txtTaxCode.Text);
                if (exists)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.TaxCodeExistsOnServer, ResourcesRest.MsgBtnOk);
                        return;
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(ResourcesRest.Save, ResourcesRest.FailContactServer, ResourcesRest.MsgBtnOk);
                    return;
                });
                return;
            }

            try
            {
                _CurrentCustomer.CompanyName = txtCompanyName.Text;
                _CurrentCustomer.Profession = Proffesion.Text;
                _CurrentCustomer.Code = txtTaxCode.Text;
                _CurrentCustomer.IsActive = true;
                _CurrentCustomer.SetOwner(App.Owner);
                _CurrentCustomer.Trader.TaxCode = txtTaxCode.Text;
                _CurrentCustomer.VatLevelOid = _CurrentCustomer.DefaultAddress.VatLevelOid;
                _CurrentCustomer.VatLevel = _CurrentCustomer.DefaultAddress.VatLevel;
                _CurrentCustomer.Trader.TaxOffice = ObservableTaxOffice.Where(x => x.Oid == _CurrentCustomer.Trader.TaxOfficeLookUpOid).FirstOrDefault()?.Description ?? "";
                _CurrentCustomer.Trader.Code = _CurrentCustomer.Code;

                if (string.IsNullOrEmpty(_CurrentCustomer.Trader.Code) || string.IsNullOrEmpty(_CurrentCustomer.Trader.TaxCode) || string.IsNullOrEmpty(_CurrentCustomer.CompanyName)
                    || _CurrentCustomer.DefaultAddress.VatLevelOid == Guid.Empty || _CurrentCustomer.Trader.TaxOfficeLookUpOid == Guid.Empty
                    || _CurrentCustomer.DefaultAddress.VatLevelOid == null || _CurrentCustomer.Trader.TaxOfficeLookUpOid == null || string.IsNullOrEmpty(_CurrentCustomer.Profession))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.Save, ResourcesRest.PleaseFillInAllRequiredFields, ResourcesRest.MsgBtnOk);
                        return;
                    });
                    return;
                }

                Guid customerOid = Guid.NewGuid();
                _CurrentCustomer.Oid = customerOid;
                Guid vatLevelOid = _CurrentCustomer.DefaultAddress.VatLevelOid;
                Guid AddressTypeOid = _CurrentCustomer.DefaultAddress.AddressTypeOid;
                _CurrentCustomer.DefaultAddress.IsDefault = true;
                _CurrentCustomer.VatLevelOid = vatLevelOid;
                HttpResponseMessage response = await ApiHelper.PostCustomer(_CurrentCustomer);
                if (response.IsSuccessStatusCode)
                {
                    Address postAddress = _CurrentCustomer.DefaultAddress;
                    _CurrentCustomer = await ApiHelper.GetCustomer(customerOid);
                    if (_CurrentCustomer != null && _CurrentCustomer.Oid == customerOid && _CurrentCustomer.Trader.Oid != null && _CurrentCustomer.Trader.Oid != Guid.Empty)
                    {
                        _CurrentCustomer.DefaultAddress = postAddress;
                        _CurrentCustomer.DefaultAddress.Trader = _CurrentCustomer.Trader;
                        _CurrentCustomer.DefaultAddress.TraderOid = _CurrentCustomer.Trader?.Oid ?? _CurrentCustomer.TraderOid;
                        _CurrentCustomer.DefaultAddress.Profession = _CurrentCustomer.Profession;
                        App.DbLayer.InsertNewRecord<Trader>(_CurrentCustomer.Trader);
                        App.DbLayer.InsertNewRecord<Customer>(_CurrentCustomer);
                        response = await ApiHelper.PostAddress(_CurrentCustomer.DefaultAddress);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseAddress = await response.Content.ReadAsStringAsync();
                            _CurrentCustomer.DefaultAddress = JsonConvert.DeserializeObject<Address>(responseAddress);
                            _CurrentCustomer.DefaultAddress.TraderOid = _CurrentCustomer.TraderOid;
                            _CurrentCustomer.DefaultAddress.VatLevelOid = _CurrentCustomer.VatLevelOid;
                            _CurrentCustomer.DefaultAddress.Profession = _CurrentCustomer.Profession;
                            _CurrentCustomer.DefaultAddress.AddressTypeOid = AddressTypeOid;
                            _CurrentCustomer.DefaultAddress.VatLevelOid = vatLevelOid;
                            _CurrentCustomer.DefaultAddress.IsDefault = true;
                            App.DbLayer.InsertNewRecord<Address>(_CurrentCustomer.DefaultAddress);
                            _CurrentCustomer.DefaultAddressOid = _CurrentCustomer.DefaultAddress.Oid;
                            App.DbLayer.Update<Customer>(_CurrentCustomer);
                            resultmessage = ResourcesRest.AddCustomerMsgSuccesfullSaved;
                        }
                        else
                        {
                            resultmessage = ResourcesRest.FailureAdd;
                        }
                    }
                    else
                    {
                        resultmessage = ResourcesRest.FailureAdd;
                    }
                }
                else
                {
                    resultmessage = ResourcesRest.FailureAdd;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                resultmessage = ResourcesRest.sendExceptionMsg + " " + ex.Message;
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                DisplayAlert(ResourcesRest.Save, resultmessage, ResourcesRest.MsgBtnOk);
            });
            if (resultmessage == ResourcesRest.AddCustomerMsgSuccesfullSaved)
            {
                await ClosePage();
            }
        }

        async void OnCancel(object sender, EventArgs eventArguments)
        {
            await ClosePage();
        }

        private async Task ClosePage()
        {
            await Task.Delay(1);
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopAsync(true);
            });
        }

        /// <summary>
        /// ADDRESS STACK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>   
        #region
        async void ShowAddressForm(object sender, EventArgs eventArguments)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            IReadOnlyList<Page> stack = Navigation.NavigationStack;
            if (stack[stack.Count - 1].GetType() != typeof(AddCustomerAddress))
            {
                await Navigation.PushModalAsync(new AddCustomerAddress(ref _CurrentCustomer), true);
            }
            UserDialogs.Instance.HideLoading();
        }
        #endregion


        /// <summary>
        /// TAX OFFICE STACK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #region
        async void ShowTaxOfficeList(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            TaxOfficeStack.IsVisible = true;
            Stack.IsVisible = false;
            await Task.Delay(1000);
            UserDialogs.Instance.HideLoading();
        }
        async void HideTaxOfficeForm(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            TaxOfficeStack.IsVisible = false;
            Stack.IsVisible = true;
            await Task.Delay(1000);
            UserDialogs.Instance.HideLoading();
        }
        async Task SearchTaxOffice(string searchFilter)
        {
            try
            {
                searchFilter = searchFilter == null ? "" : searchFilter;
                Device.BeginInvokeOnMainThread(() =>
                {
                    TaxOfficeList.ItemsSource = ObservableTaxOffice.Where(x => x.Description.ToUpper().Contains(searchFilter.ToUpper()));
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        async void AddTaxOfficeToCustomer(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    _CurrentCustomer.Trader.TaxOfficeLookUpOid = Guid.Empty;
                    return;
                }
                TaxOffice currentTaxOffice = ObservableTaxOffice.Where(Oid => Oid.Oid.Equals(menuItem.CommandParameter))?.FirstOrDefault() ?? null;
                _CurrentCustomer.Trader.TaxOffice = currentTaxOffice?.Description;
                _CurrentCustomer.Trader.TaxOfficeLookUpOid = currentTaxOffice?.Oid ?? Guid.Empty;
                CurrentTaxOffice.Text = currentTaxOffice?.Description ?? "";
                TaxOfficeStack.IsVisible = false;
                Stack.IsVisible = true;
            }
            catch (Exception ex)
            {
                CurrentTaxOffice.Text = "";
                _CurrentCustomer.Trader.TaxOffice = "";
                _CurrentCustomer.Trader.TaxOfficeLookUpOid = Guid.Empty;
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                TaxOfficeStack.IsVisible = false;
                Stack.IsVisible = true;
            }
        }
        async void btnSearch(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(async () =>
                {
                    await SearchTaxOffice(searchOffice.Text);
                });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        protected async void SearchByKeyboard(object sender, EventArgs e)
        {
            try
            {
                if (searchOffice.Text != null && searchOffice.Text != "")
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                    await Task.Run(async () =>
                    {
                        await SearchTaxOffice(searchOffice.Text);
                    });
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        #endregion


        /// <summary>
        /// CUSTOMER PAGE SETUP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #region
        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblAddNewCustomer.Text = ResourcesRest.AddCustomerlblAddNewCustomer;
            txtCompanyName.Placeholder = ResourcesRest.CompanyName;
            txtTaxCode.Placeholder = ResourcesRest.AddCustomerTxtTaxCode;
            txtCode.Placeholder = ResourcesRest.AddCustomerTxtCode;
            btnSaveCustomer.Text = ResourcesRest.Save;

            btnCancel.Text = ResourcesRest.AddCustomerBtnCancel;
            btnCheck.Text = ResourcesRest.AddCustomerBtnCheck;
            btnShowAddressForm.Text = ResourcesRest.AddCustomerBtnSaveNewAddress;

            lblTaxCode.Text = ResourcesRest.TaxCode;
            lblCode.Text = ResourcesRest.Code;
            lblCompanyName.Text = ResourcesRest.CompanyName;
            lblArea.Text = ResourcesRest.Area;
            lblTaxOffice.Text = ResourcesRest.TaxOffice;
            btnCancelTaxOfficeToCustomer.Text = ResourcesRest.Cancel;
            btnShowTaxOfficeList.Text = ResourcesRest.AddTaxOffice;
            lblCurrentAddress.Text = ResourcesRest.Address;
            lblCurrentTaxOffice.Text = ResourcesRest.TaxOffice;
            searchOffice.Placeholder = ResourcesRest.SearchTaxOffice;
            lblProffesion.Text = ResourcesRest.profession;
            Proffesion.Placeholder = ResourcesRest.profession;
        }

        private void InitialiseSelectOptions()
        {
            ObservableTaxOffice = new ObservableCollection<TaxOffice>(App.DbLayer.GetTaxOffices());
            TaxOfficeList.ItemsSource = ObservableTaxOffice;

        }

        protected override void OnAppearing()
        {
            if (_CurrentCustomer.DefaultAddress == null)
            {
                _CurrentCustomer.DefaultAddress = new Address();
            }
            CurrentAddress.Text = _CurrentCustomer.DefaultAddress.Description(App.DbLayer);
            base.OnAppearing();
        }
        #endregion

        #region
        /// <summary>
        /// FOCUS EVENTS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void searchOfficeUnFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void searchOfficeFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        public void TaxCodeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void TaxCodeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        public void CompanyNameUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void CompanyNameFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        public void CodeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void CodeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void ProffesionUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void ProffesionFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }


        #endregion


    }
}
