using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages.Maps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutesPage : ContentPage
    {
        private ObservableCollection<Route> Routes = new ObservableCollection<Route>();
        public RoutesPage()
        {
            InitializeComponent();
            InitiallizeControllers();

            var data = App.DbLayer.GetAllRoutes();
            RouteListView.ItemsSource = data;
        }


        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblStoreRoutes.Text = ResourcesRest.StoredRoutes;
            lblName.Text = ResourcesRest.Name;
            lblDistance.Text = ResourcesRest.Distance;
        }

        protected void LoadRoute(object sender, EventArgs e)
        {
            try
            {
                ListView listView = sender as ListView;
                Route route = (Route)listView.SelectedItem;
                if (route != null)
                {
                    var master = this.Parent as MapMaster;
                    master.MoveToMapPage(route);
                }

            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

    }
}