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
    public partial class TabCustomerLocationPage : ContentPage
    {
        ObservableCollection<AddressPresentation> obsevableAddresses = new ObservableCollection<AddressPresentation>();
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        public TabCustomerLocationPage(Customer customer)
        {
            InitializeComponent();
            InitiallizeControllers();
            lblDetail.Text = customer.CompanyName;
            LoadAddresses(customer);
        }
        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;

        }
        private async void LoadAddresses(Customer customer)
        {
            try
            {
                DatabaseLayer dbLayer = DependencyService.Get<ICrossPlatformMethods>().GetDataBaseLayer();
                foreach (var address in customer.Trader.Addresses)
                {
                    string addressDescription = address.Description(dbLayer);
                    obsevableAddresses.Add(new AddressPresentation() { Oid = address.Oid, AddressDescription = addressDescription, Lat = address.Latitude.ToString(), Lng = address.Longitude.ToString() });
                }
                AddressesListView.ItemsSource = obsevableAddresses;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
    }
}