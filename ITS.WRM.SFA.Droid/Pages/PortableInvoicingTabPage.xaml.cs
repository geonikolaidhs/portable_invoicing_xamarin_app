using ITS.WRM.SFA.Droid.Pages.PortableInvoicingChildViews;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PortableInvoicingTabPage : TabbedPage
    {
        public PortableInvoicingTabPage()
        {
            InitializeComponent();
            InitiallizeControllers();
            Children.Add(new TabStockItems() { Title = ResourcesRest.StockItems });
            Children.Add(new ΤabReceiveDocuments() { Title = ResourcesRest.UpdateItemsStock });
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
        }
    }
}