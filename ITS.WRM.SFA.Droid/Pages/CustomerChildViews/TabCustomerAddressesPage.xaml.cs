using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabCustomerAddressesPage : ContentPage
    {

        ObservableCollection<AddressPresentation> obsevableAddresses = new ObservableCollection<AddressPresentation>();
        private Address CurrentAddress = null;
        List<Address> adresses = new List<Address>();
        private Customer CurrentCustomer { get; set; }
        List<AddressType> ListAddressType = App.DbLayer.GetAddressTypes();
        List<VatLevel> ListVatLevel = App.VatLevels;

        public TabCustomerAddressesPage(Customer customer)
        {
            InitializeComponent();
            InitiallizeControllers();
            CurrentCustomer = customer;
            if (CurrentCustomer.Trader == null && CurrentCustomer.TraderOid != null && CurrentCustomer.TraderOid != Guid.Empty)
            {
                CurrentCustomer.Trader = App.DbLayer.GetById<Trader>(CurrentCustomer.TraderOid);
            }
            lblDetail.Text = customer.CompanyName;
            //LoadAddresses(customer);
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (CurrentCustomer != null)
            {
                LoadAddresses(CurrentCustomer);
            }
        }

        private async void LoadAddresses(Customer customer)
        {
            try
            {
                adresses = App.DbLayer.GetAddressByTrader(customer.TraderOid) ?? new List<Address>();
                obsevableAddresses.Clear();
                foreach (var address in adresses)
                {
                    if (address.City != null)
                    {
                        string addressDescription = address.Description(App.DbLayer);
                        obsevableAddresses.Add(new AddressPresentation() { Oid = address.Oid, AddressDescription = addressDescription });
                    }
                }
                AddressesListView.ItemsSource = obsevableAddresses;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        protected async void editAddress(object sender, EventArgs e)
        {
            try
            {

                MenuItem item = (MenuItem)sender;
                Guid AddrOid = adresses.Where(xx => xx.Oid.Equals(item.CommandParameter))?.First()?.Oid ?? Guid.Empty;
                if (AddrOid == Guid.Empty)
                {
                    return;
                }
                CurrentAddress = App.DbLayer.GetAddressById(AddrOid);
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(AddCustomerAddress))
                {
                    await Navigation.PushModalAsync(new EditCustomerAddress(ref CurrentAddress, CurrentCustomer), true);
                }
                UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

    }
}
