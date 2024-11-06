using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerEditPage : ContentPage
    {
        private ObservableCollection<TaxOffice> ObservableTaxOffice = new ObservableCollection<TaxOffice>();
        private Customer _CurrentCustomer;

        public CustomerEditPage(Customer customer, bool EditFlag)
        {
            InitializeComponent();
            _CurrentCustomer = customer;
            this.BindingContext = _CurrentCustomer;
            InitiallizeControllers();
            lblDetail.Text = customer.CompanyName;
            EditStack.IsVisible = EditFlag;
            DetailStack.IsVisible = !EditFlag;
            ObservableTaxOffice = new ObservableCollection<TaxOffice>(App.DbLayer.GetTaxOffices());
            TaxOfficeList.ItemsSource = ObservableTaxOffice;
            _CurrentCustomer.GetDefaultAddress(App.DbLayer);
            txtAddress.Detail = _CurrentCustomer.DefaultAddress?.Description(App.DbLayer) ?? "";
            txtTaxOffice.Detail = ObservableTaxOffice.ToList().Where(x => x.Oid == _CurrentCustomer?.Trader.TaxOfficeLookUpOid).FirstOrDefault()?.Description ?? "";
            TaxOffice.Text = txtTaxOffice.Detail;
            TaxOfficeStack.IsVisible = false;
        }

        async public void OnSaveCustomer(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            if (_CurrentCustomer.Trader != null && _CurrentCustomer.Trader.TaxCode != txtTaxCode.Detail)
                try
                {
                    bool exists = await TaxCodeExistsOnServer(txtTaxCode.Detail);
                    if (exists)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            UserDialogs.Instance.HideLoading();
                            DisplayAlert(ResourcesRest.Save, ResourcesRest.TaxCodeExistsOnServer, ResourcesRest.MsgBtnOk);
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
                _CurrentCustomer.CompanyName = CompanyName.Text;
                _CurrentCustomer.Code = Code.Text;
                _CurrentCustomer.Profession = Profession.Text;
                if (_CurrentCustomer.Trader == null)
                {
                    _CurrentCustomer.Trader = App.DbLayer.GetTraderById(_CurrentCustomer.TraderOid);
                }
                if (_CurrentCustomer.Trader != null)
                {
                    _CurrentCustomer.Trader.TaxCode = TaxCode.Text;
                    _CurrentCustomer.Trader.Addresses = App.DbLayer.GetAddressByTrader(_CurrentCustomer.TraderOid);
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert("Αποθήκευση", "Δεν βρέθηκε Συναλλασόμενος για τον Πελάτη", "Οκ");
                    return;
                }

                HttpResponseMessage response = await ApiHelper.EditCustomer(_CurrentCustomer);
                if (response.IsSuccessStatusCode)
                {
                    App.DbLayer.Update<Trader>(_CurrentCustomer.Trader);
                    App.DbLayer.Update(_CurrentCustomer);
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert("Αποθήκευση", "Η επεξεργασία Πελάτη ολοκληρώθηκε επιτυχώς.", "Οκ");
                }
                else
                {
                    await DisplayAlert("Αποθήκευση", "Υπήρξαν σφάλματα κατά τη διαδικασία αποθήκευσης των αλλαγών", "Οκ");
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
            UserDialogs.Instance.HideLoading();
        }

        async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
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
                txtTaxOffice.Text = currentTaxOffice?.Description ?? "";
                TaxOffice.Text = currentTaxOffice?.Description ?? "";
                TaxOfficeStack.IsVisible = false;
                EditStack.IsVisible = true;
            }
            catch (Exception ex)
            {
                txtTaxOffice.Text = "";
                _CurrentCustomer.Trader.TaxOffice = "";
                _CurrentCustomer.Trader.TaxOfficeLookUpOid = Guid.Empty;
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                TaxOfficeStack.IsVisible = false;
            }
        }

        async void ShowTaxOfficeList(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            EditStack.IsVisible = false;
            TaxOfficeStack.IsVisible = true;
            await Task.Delay(1000);
            UserDialogs.Instance.HideLoading();
        }

        async void HideTaxOfficeForm(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            TaxOfficeStack.IsVisible = false;
            EditStack.IsVisible = true;
            await Task.Delay(1000);
            UserDialogs.Instance.HideLoading();
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

        public async void CheckCustomerOnServer(object sender, EventArgs eventArguments)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.MsgSendingData, MaskType.Black);
                if (!DependencyService.Get<ICrossPlatformMethods>().IsConnected())
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.msgNotConnectionFound, ResourcesRest.MsgBtnOk);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txtTaxCode.Detail))
                {
                    bool exists = await TaxCodeExistsOnServer(txtTaxCode.Detail);
                    if (exists)
                    {
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.MsgAddOrEditCustomer, ResourcesRest.MsgBtnOk);
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

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            txtCompanyName.Text = ResourcesRest.CustomerDetailLblCompanyName;
            txtTaxCode.Text = ResourcesRest.CustomerDetailLblTaxCode;
            txtCode.Text = ResourcesRest.CustomerDetailLblCode;
            lblCompanyName.Text = ResourcesRest.CustomerDetailLblCompanyName;
            lblTaxCode.Text = ResourcesRest.CustomerDetailLblTaxCode;
            lblCode.Text = ResourcesRest.CustomerDetailLblCode;
            lblTaxOffice.Text = ResourcesRest.TaxOffice;
            lblTaxOffice2.Text = ResourcesRest.TaxOffice;
            btnSaveCustomer.Text = ResourcesRest.Save;
            btnCancel.Text = ResourcesRest.CustomerDetailBtnCancel;
            lblEdit.Text = ResourcesRest.EditCustomer;
            txtCompanyName.Text = ResourcesRest.CompanyName;
            btnShowTaxOfficeList.Text = ResourcesRest.EditTaxOffice;
            txtTaxOffice.Text = ResourcesRest.TaxOffice;
            txtAddress.Text = ResourcesRest.Address;
            lblProffesion.Text = ResourcesRest.profession;
            txtProfession.Text = ResourcesRest.profession;
        }

        public void CompanyNameFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void CompanyNameUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void TaxCodeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void TaxCodeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void CodeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void CodeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void TaxOfficeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void TaxOfficeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

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
    }
}
