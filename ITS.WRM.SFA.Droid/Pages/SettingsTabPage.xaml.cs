using ITS.WRM.SFA.Droid.Pages.SettingsChiledViews;
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
    public partial class SettingsTabPage : TabbedPage
    {
        public SettingsTabPage(string PageComeFrom)
        {
            InitializeComponent();
            InitiallizeControllers();
            Children.Add(new SettingsPage(PageComeFrom) { Title = ResourcesRest.SettingsLblSettings });
            Children.Add(new VersioningPage() { Title = "Versioning" });
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");

        }
    }
}