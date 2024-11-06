using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Specialized;
using System.Diagnostics;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerTabDetailPage : TabbedPage
    {
        public CustomerTabDetailPage(Customer customer)
        {
            InitializeComponent();
            InitiallizeControllers();

            Children.Add(new CustomerEditPage(customer, false) { Title = ResourcesRest.TabCustomerDetails });
            Children.Add(new TabCustomerAddressesPage(customer) { Title = ResourcesRest.TabCustomerAddresses });
            Children.Add(new TabCustomerOrderPage(customer) { Title = ResourcesRest.TabCustomerOrders });
            //Children.Add(new TabCustomerLocationPage(customer) { Title = ResourcesRest.TabCustomerLocation });
        }
        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
        }
    }
}