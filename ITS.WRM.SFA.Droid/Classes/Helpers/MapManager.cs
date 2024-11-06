

using System;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public class MapManager
    {
        private Map _Map;

        public MapManager(Map map)
        {
            _Map = map;
        }

        public void SetMapLocationByPosition(Position pos)
        {
            if (_Map == null)
            {
                throw new System.Exception("Map is not Initialised");
            }


        }

        public void SetMapLocationByAddress(string address)
        {
            if (_Map == null)
            {
                throw new System.Exception("Map is not Initialised");
            }
        }

        public Map CreateNewMap(StackLayout MapStack)
        {
            _Map = new Map
            {
                HasScrollEnabled = true,
                HasZoomEnabled = true,
                IsShowingUser = true,
                VerticalOptions = LayoutOptions.FillAndExpand,
                MapType = MapType.Hybrid
            };

            var street = new Button { Text = "Street" };
            street.WidthRequest = 80;

            var hybrid = new Button { Text = "Hybrid" };
            hybrid.WidthRequest = 80;

            var satellite = new Button { Text = "Satellite" };
            satellite.WidthRequest = 80;

            street.Clicked += HandleClicked;
            hybrid.Clicked += HandleClicked;
            satellite.Clicked += HandleClicked;

            var segments = new StackLayout
            {
                Spacing = 30,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = { street, hybrid, satellite }
            };

            MapStack.Children.Add(_Map);
            MapStack.Children.Add(segments);

            return _Map;
        }

        void HandleClicked(object sender, EventArgs e)
        {
            var b = sender as Button;
            switch (b.Text)
            {
                case "Street":
                    _Map.MapType = MapType.Street;
                    break;
                case "Hybrid":
                    _Map.MapType = MapType.Hybrid;
                    break;
                case "Satellite":
                    _Map.MapType = MapType.Satellite;
                    break;
            }
        }


    }
}
