using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages.Maps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapMaster : TabbedPage
    {
        protected List<CustomPin> AvailiablePins = null;
        protected Position CurrentPosition;


        public MapMaster(List<CustomPin> pins, Position currentPosition, string apikey)
        {
            AvailiablePins = pins;
            CurrentPosition = currentPosition;
            InitializeComponent();
            InitiallizeControllers();
            Children.Add(new MapPage(CurrentPosition, AvailiablePins, apikey) { Title = ResourcesRest.RouteMap, Icon = "route.png" });
            //Children.Add(new RoutesPage() { Title = ResourcesRest.StoredRoutes, Icon = "route.png" });
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
        }

        public void MoveToMapPage(Route route)
        {
            this.CurrentPage = Children[0];
        }
    }
}