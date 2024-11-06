using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using XLabs.Forms.Controls;

namespace ITS.WRM.SFA.Droid.Pages.Maps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private Map _Map = null;
        private List<CustomPin> AllPinsList = null;
        private Position CurrentPosition;
        private ObservableCollection<CustomPin> AvailiablePins = null;
        private Route Route = null;
        private ObservableCollection<CustomPin> SelectedPins = null;
        private ObservableCollection<CustomPin> RouteListViewDataSource = null;
        private CustomPin SelectedPin = null;
        private bool IsNewRoute = true;
        private string ApiKey;
        private Assembly Assembly = null;
        Color CheckBoxTextColor = Color.WhiteSmoke;
        protected GridLength FirstRow = new GridLength(40, GridUnitType.Star);
        protected GridLength SecondRow = new GridLength(60, GridUnitType.Star);

        protected GridLength MapColumn = new GridLength(60, GridUnitType.Star);
        protected GridLength MapInfoColumn = new GridLength(40, GridUnitType.Star);
        private List<MapType> MapTypes = new List<MapType>() { MapType.Hybrid, MapType.Satellite, MapType.Street, MapType.Terrain };

        public MapPage(Position currentPosition, List<CustomPin> pins, string apikey, Route route = null, string routeName = "")
        {
            InitializeComponent();
            PageGrid.RowDefinitions[0].Height = FirstRow;
            PageGrid.RowDefinitions[1].Height = SecondRow;
            MapInfo.IsVisible = false;
            RouteListViewDataSource = new ObservableCollection<CustomPin>();
            MapGrid.ColumnDefinitions[0].Width = new GridLength(100, GridUnitType.Star);
            MapGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
            Assembly = typeof(ITS.WRM.SFA.Model.Model.SFA).GetTypeInfo().Assembly;
            SaveStack.IsVisible = false;
            InitiallizeControllers();
            CurrentPosition = currentPosition;
            AllPinsList = pins;
            ApiKey = string.IsNullOrEmpty(apikey) ? string.Empty : apikey;
            AvailiablePins = new ObservableCollection<CustomPin>(AllPinsList);
            SelectedPins = new ObservableCollection<CustomPin>(route != null ? route.RoutePins : new List<CustomPin>());
            IsNewRoute = route == null ? true : false;
            Route = route == null ? new Route() : route;
            Route.Name = routeName;
            CreateMap();
            UpdateUi(true);
            CreateCurrentLocation(CurrentPosition);

            lblRouteInfoHeader.IsVisible = false;
            lblRouteInfo.IsVisible = false;

            lblRouteInfoHeader.Text = "";
            lblRouteInfo.Text = "";

        }


        private void UpdateListLabels()
        {
            lblAvailiableAddresses.Text = ResourcesRest.AvailiableAddresses + " (" + AvailiablePins.Count + ")";
            lblSelectedAddresses.Text = ResourcesRest.SelectedAddresses + " (" + SelectedPins.Count + ")";
        }

        private void UpdateListSource()
        {
            AvailiableAddressesList.ItemsSource = AvailiablePins;
            SelectedAddressesList.ItemsSource = SelectedPins;
        }

        private void UpdateUi(bool updateMap = false)
        {
            UpdateListSource();
            UpdateListLabels();
            if (updateMap)
            {
                UpdateMap();
            }
        }

        void OnShowAddressPanelChanged(object sender, EventArgs e)
        {
            if (switchShowAddressPanel.Checked)
            {
                PageGrid.RowDefinitions[0].Height = FirstRow;
                PageGrid.RowDefinitions[1].Height = SecondRow;
                AddressPanelGrid.IsVisible = true;
            }
            else
            {
                AddressPanelGrid.IsVisible = false;
                PageGrid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                PageGrid.RowDefinitions[1].Height = new GridLength(100, GridUnitType.Star);
                _Map.HeightRequest = MapStack.Height;
            }
        }

        void OnShowRouteInfoChanged(object sender, EventArgs e)
        {
            if (switchShowRouteInfo.Checked)
            {
                MapGrid.ColumnDefinitions[0].Width = MapColumn;
                MapGrid.ColumnDefinitions[1].Width = MapInfoColumn;
                MapInfo.IsVisible = true;
            }
            else
            {
                MapInfo.IsVisible = false;
                MapGrid.ColumnDefinitions[0].Width = new GridLength(100, GridUnitType.Star);
                MapGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                _Map.WidthRequest = MapStack.Width;
            }
        }

        void ShowTrafficChanged(object sender, EventArgs e)
        {
            _Map.IsTrafficEnabled = switchShowTraffic.Checked;
        }

        private void CreateCurrentLocation(Position pos)
        {

            Pin mapPin = new Pin()
            {
                Icon = BitmapDescriptorFactory.FromStream(Assembly.GetManifestResourceStream("ITS.WRM.SFA.Model.startline.png") ?? Assembly.GetManifestResourceStream("ITS.WRM.SFA.startline.png"), id: "startline.png"),
                Type = PinType.SearchResult,
                Position = new Position(pos.Latitude, pos.Longitude),
                Label = "Your Location",
                Address = "Your Location",
                ZIndex = 99999
            };
            _Map.Pins.Add(mapPin);
        }

        private void CreateMap()
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                _Map = new Map
                {
                    IsTrafficEnabled = true,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    MapType = MapType.Hybrid
                };
                _Map.UiSettings.ZoomControlsEnabled = true;
                _Map.UiSettings.ZoomGesturesEnabled = true;
                _Map.UiSettings.ScrollGesturesEnabled = true;
                _Map.UiSettings.RotateGesturesEnabled = true;
                _Map.UiSettings.IndoorLevelPickerEnabled = false;
                _Map.UiSettings.CompassEnabled = true;
                _Map.UiSettings.MapToolbarEnabled = true;
                _Map.UiSettings.MyLocationButtonEnabled = true;
                _Map.UiSettings.TiltGesturesEnabled = false;
                _Map.MoveToRegion(MapSpan.FromCenterAndRadius(CurrentPosition, Distance.FromKilometers(4)));
                _Map.SelectedPinChanged += async (s, e) =>
                {
                    if (SelectedPin != null)
                    {
                        if (SelectedPin.InRoute)
                        {
                            SelectedPin.SetIconFromRouteRow();
                        }
                        else if (SelectedPin.InSelectedAddresses)
                        {

                            SelectedPin.SetinRouteIconColor();
                        }
                        else
                        {
                            SelectedPin.SetDefaultIconColor();
                        }
                    }
                    SelectedPin = AllPinsList.Where(x => x.Pin == e.SelectedPin).FirstOrDefault();
                    if (SelectedPin != null)
                    {
                        SelectedPin.SetSelectedIconColor();
                    }
                };
                foreach (var pin in AllPinsList)
                {
                    pin.PinLongClicked += (s, e) =>
                     {
                         if (e != null && e.CustomPin != null)
                         {
                             if (!SelectedPins.Contains(e.CustomPin))
                             {
                                 pin.InSelectedAddresses = true;
                                 SelectedPins.Add(pin);
                                 AvailiablePins.Remove(pin);
                                 pin.SetinRouteIconColor();
                                 UpdateUi(false);
                             }
                             else
                             {
                                 pin.InSelectedAddresses = false;
                                 SelectedPins.Remove(pin);
                                 AvailiablePins.Add(pin);
                                 pin.SetDefaultIconColor();
                                 pin.ClearFromRoute();
                                 UpdateUi(false);
                             }
                         }
                     };
                    _Map.Pins.Add(pin.Pin);
                }
                _Map.PropertyChanged += (sender, e) =>
                {
                    Debug.WriteLine(e.PropertyName + " just changed!");
                    if (e.PropertyName == "VisibleRegion" && _Map.VisibleRegion != null)
                        CalculateBoundingCoordinates(_Map.VisibleRegion);
                };
                MapStack.Children.Add(_Map);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        void HandleClicked(object sender, EventArgs e)
        {
            switch ((sender as Button).Text)
            {
                case "Street":
                    _Map.MapType = MapType.Street;
                    break;
                case "Hybrid":
                    _Map.MapType = MapType.Hybrid;
                    break;
                case "Terrain":
                    _Map.MapType = MapType.Terrain;
                    break;
            }
        }

        static void CalculateBoundingCoordinates(MapSpan region)
        {
            //var center = region.Center;
            //var halfheightDegrees = region.LatitudeDegrees / 2;
            //var halfwidthDegrees = region.LongitudeDegrees / 2;

            //var left = center.Longitude - halfwidthDegrees;

            //var right = center.Longitude + halfwidthDegrees;
            //var top = center.Latitude + halfheightDegrees;
            //var bottom = center.Latitude - halfheightDegrees;

            //if (left < -180) left = 180 + (180 + left);
            //if (right > 180) right = (right - 180) - 180;

            //var width = Math.Abs(left - right);
            //var height = Math.Abs(bottom - top);
            //var rect = new Rectangle(left, top, width, height);

            //Left = left;
            //Right = right;
            //Top = top;
            //Bottom = bottom;

            //Position southWest = new Position(bottom, left);
            //Position nortEast = new Position(top, right);

            //MapBounds = new Bounds(southWest, nortEast);


            //Debug.WriteLine("Bounding box:");
            //Debug.WriteLine("                    " + top);
            //Debug.WriteLine("  " + left + "                " + right);
            //Debug.WriteLine("                    " + bottom);
        }

        protected async void CreateRoute(object sender, EventArgs args)
        {
            try
            {
                UserDialogs.Instance.ShowLoading();
                if (SelectedPins == null || SelectedPins.Count < 1)
                {
                    await DisplayAlert(ResourcesRest.sendExceptionMsg, "Please insert at least one destination", ResourcesRest.MsgBtnOk);
                }
                Route = new Route();
                List<CustomPin> list = new List<CustomPin>(SelectedPins.ToList());
                if (list.Select(x => x.Pin.Position).ToList().Contains(CurrentPosition))
                {
                    list.Remove(list.Where(x => x.Pin.Position == CurrentPosition).FirstOrDefault());
                }
                Route.StartLat = CurrentPosition.Latitude;
                Route.StartLot = CurrentPosition.Longitude;
                Position fromPosition = CurrentPosition;
                CustomPin closestPin = null;
                int routeIndex = 0;
                int index = 0;
                while (list.Count > 0)
                {
                    try
                    {
                        if (index == 0)
                        {
                            fromPosition = CurrentPosition;
                        }
                        else
                        {
                            if (closestPin != null)
                            {
                                fromPosition = closestPin.Pin.Position;
                            }
                        }
                        //closestPin = LocationManager.GetClosestDistance(fromPosition, list);
                        //var stps = await GetRouteSteps(closestPin, fromPosition);
                        var sortedList = await GetSortedListByDistance(list, fromPosition);
                        closestPin = sortedList.FirstOrDefault();
                        closestPin.RowNumber = index + 1;
                        closestPin.InRoute = true;
                        Route.RoutePins.Add(closestPin);
                        var steps = closestPin.DirectionsApiResponse.routes[0].legs[0].steps;
                        foreach (var step in steps)
                        {
                            //if (routeIndex == 0)
                            //{
                            RouteStep startPoint = new RouteStep(routeIndex, new Position(step.start_location.lat, step.start_location.lng), step.polyline.points);
                            Route.RouteSteps.Add(startPoint);
                            if (!Route.RouteSteps.Select(x => x.Position).ToList().Contains(startPoint.Position))
                            {
                                routeIndex++;
                                // Route.RouteSteps.Add(startPoint);
                            }
                            else
                            {
                                if (Route.RouteSteps.Where(x => x.Position == startPoint.Position).LastOrDefault().Number != (routeIndex - 1))
                                {
                                    routeIndex++;
                                    //Route.RouteSteps.Add(startPoint);
                                }
                            }
                            //}
                            RouteStep endPoint = new RouteStep(routeIndex, new Position(step.end_location.lat, step.end_location.lng), step.polyline.points);
                            routeIndex++;
                            Route.RouteSteps.Add(endPoint);
                        }
                        list.Remove(closestPin);
                        index++;
                        var find = SelectedPins.Where(x => x == closestPin).FirstOrDefault();
                        if (find != null)
                        {
                            SelectedPins.Remove(find);
                            SelectedPins.Add(closestPin);
                        }
                        find = Route.RoutePins.Where(x => x == closestPin).FirstOrDefault();
                        if (find != null)
                        {
                            Route.RoutePins.Remove(find);
                            Route.RoutePins.Add(closestPin);
                        }
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                }
                CreateLineToMap(_Map, Route);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        public async Task<List<step>> GetRouteSteps(CustomPin pin, Position fromPosition)
        {
            string apiUrl = SFAConstants.GOOGLE_MAP_DIRECTIONS_API_URL + @"?&origin=" + fromPosition.Latitude.ToString().Replace(",", ".")
            + "," + fromPosition.Longitude.ToString().Replace(",", ".") + "&destination={0}" + @"&key=" + ApiKey;
            List<step> steps = new List<step>();
            try
            {
                string location = pin.Latitude.ToString().Replace(",", ".") + "," + pin.Longitude.ToString().Replace(",", ".");
                string url = string.Format(apiUrl, location);
                string content = string.Empty;

                using (HttpClient httpClient = new HttpClient(new NativeMessageHandler()))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    content = await response.Content.ReadAsStringAsync();
                    var resp = JsonConvert.DeserializeObject<DirectionsApiResponse>(content);
                    if (resp.routes.Count > 0)
                    {
                        if (resp.routes[0].legs.Count > 0)
                        {
                            pin.Distance = resp.routes[0].legs[0].distance;
                            pin.Duration = resp.routes[0].legs[0].duration;
                            pin.DirectionsApiResponse = resp;
                            steps = resp.routes[0].legs[0].steps;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }

            return steps;
        }

        public async Task<List<CustomPin>> GetSortedListByDistance(List<CustomPin> listPins, Position fromPosition)
        {
            string apiUrl = SFAConstants.GOOGLE_MAP_DIRECTIONS_API_URL + @"?&origin=" + fromPosition.Latitude.ToString().Replace(",", ".")
            + "," + fromPosition.Longitude.ToString().Replace(",", ".") + "&destination={0}" + @"&key=" + ApiKey;

            foreach (var pin in listPins)
            {
                try
                {
                    string location = pin.Latitude.ToString().Replace(",", ".") + "," + pin.Longitude.ToString().Replace(",", ".");
                    string url = string.Format(apiUrl, location);
                    string content = string.Empty;

                    using (HttpClient httpClient = new HttpClient(new NativeMessageHandler()))
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(10);
                        HttpResponseMessage response = await httpClient.GetAsync(url);
                        content = await response.Content.ReadAsStringAsync();
                        var resp = JsonConvert.DeserializeObject<DirectionsApiResponse>(content);
                        if (resp.routes.Count > 0)
                        {
                            if (resp.routes[0].legs.Count > 0)
                            {
                                pin.Distance = resp.routes[0].legs[0].distance;
                                pin.Duration = resp.routes[0].legs[0].duration;
                                pin.DirectionsApiResponse = resp;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }
            }
            return listPins.OrderBy(x => x.Distance.value).ToList();
        }

        public void CreateLineToMap(Map map, Route route)
        {
            try
            {
                RouteListViewDataSource = new ObservableCollection<CustomPin>();
                foreach (var pin in route.RoutePins.OrderByDescending(x => x.RowNumber))
                {
                    RouteListViewDataSource.Add(pin);
                }
                map.Polylines.Clear();
                Route.RoutePolyLine = new Polyline();
                foreach (var step in route.RouteSteps.OrderBy(x => x.Number))
                {
                    Route.RoutePolyLine.Positions.Add(step.Position);
                }
                Route.RoutePolyLine.IsClickable = true;
                Route.RoutePolyLine.StrokeColor = Color.Blue;
                Route.RoutePolyLine.StrokeWidth = 5f;
                Route.RoutePolyLine.Tag = "ROUTE";
                map.Polylines.Add(Route.RoutePolyLine);
                UpdateMap();
                Route.RoutePins.OrderBy(x => x.RowNumber).LastOrDefault().Pin.Icon = BitmapDescriptorFactory.FromStream(Assembly.GetManifestResourceStream("ITS.WRM.SFA.Model.finish.png") ?? Assembly.GetManifestResourceStream("ITS.WRM.SFA.finish.png"), id: "finish.png");
                _Map.SelectedPin = null;
                lblRouteInfoHeader.IsVisible = true;
                lblRouteInfo.IsVisible = true;
                lblRouteInfoHeader.Text = ResourcesRest.RouteInfo;
                lblRouteInfo.Text = "Total Distance : " + Math.Round((Route.TotalDistance / 1000), 2, MidpointRounding.AwayFromZero) + "Km " + Environment.NewLine + " Total Duration : " + Math.Round((Route.TotalTime / 60), 2) + "Mins";
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        protected void SaveRoute(object sender, EventArgs args)
        {
            SaveStack.IsVisible = true;
        }

        protected async void OnSaveName(object sender, EventArgs args)
        {
            try
            {
                Route.Name = txtRouteName.Text;
                if (string.IsNullOrEmpty(Route.Name))
                {
                    await DisplayAlert(ResourcesRest.sendExceptionMsg, ResourcesRest.PleaseFillInAllRequiredFields, ResourcesRest.MsgBtnOk);
                    return;
                }
                App.DbLayer.InsertOrReplaceObj<Route>(Route);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                SaveStack.IsVisible = false;
            }
        }
        protected void OnCancelName(object sender, EventArgs args)
        {
            SaveStack.IsVisible = false;
        }

        protected async void AvailiableAddressesSearchBarTextChanged(object sender, EventArgs args)
        {
            await SearchOnList(AvailiablePins, AvailiableAddressesSearchBar.Text, AvailiableAddressesList);
        }

        protected void OnAvailiableAddressesSearchBarFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.Text = string.Empty;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        protected void OnAvailiableAddressesSearchBarUnFocused(object sender, EventArgs args)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void FocusOnMap(object sender, EventArgs args)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem == null)
            {
                return;
            }
            Guid oid = (Guid)menuItem.CommandParameter;
            var pin = AllPinsList.Where(x => x.Oid == oid).FirstOrDefault();
            if (pin != null && pin.Pin != null)
            {
                _Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(pin.Latitude, pin.Longitude), Distance.FromMeters(100)));
                _Map.SelectedPin = pin.Pin;
            }
        }

        protected void AddToRoute(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            Guid.TryParse(btn.CommandParameter.ToString(), out Guid oid);
            if (oid != Guid.Empty)
            {
                var pin = AvailiablePins.Where(x => x.Oid == oid).FirstOrDefault();
                if (pin != null)
                {
                    pin.InSelectedAddresses = true;
                    SelectedPins.Add(pin);
                    AvailiablePins.Remove(pin);
                    pin.SetinRouteIconColor();
                }
                UpdateUi(false);
            }
        }

        protected void RemoveFromRoute(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            Guid.TryParse(btn.CommandParameter.ToString(), out Guid oid);
            if (oid != Guid.Empty)
            {

                var pin = SelectedPins.Where(x => x.Oid == oid).FirstOrDefault();
                if (pin != null)
                {
                    if (Route.RoutePins.Contains(pin))
                    {
                        ClearRoute();
                    }

                    pin.InSelectedAddresses = false;
                    SelectedPins.Remove(pin);
                    AvailiablePins.Add(pin);
                    pin.SetDefaultIconColor();
                }
                UpdateUi(false);
            }
        }

        private void ClearRoute()
        {
            try
            {
                lblRouteInfoHeader.IsVisible = false;
                lblRouteInfo.IsVisible = false;

                lblRouteInfoHeader.Text = "";
                lblRouteInfo.Text = "";

                if (_Map.Polylines.Contains(Route.RoutePolyLine))
                {
                    _Map.Polylines.Remove(Route.RoutePolyLine);
                }
                foreach (CustomPin pin in Route.RoutePins)
                {
                    pin.ClearFromRoute();
                }
                foreach (CustomPin pin in SelectedPins)
                {
                    pin.ClearFromRoute();
                }
                Route.RoutePolyLine = new Polyline();
                Route = new Route();
                UpdateMap();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        private void UpdateMap()
        {
            //_Map.Pins.Clear();
            foreach (var pin in AvailiablePins.Where(x => x.Oid != Guid.Empty))
            {
                pin.SetDefaultIconColor();
            }
            foreach (var pin in SelectedPins.Where(x => x.Oid != Guid.Empty))
            {
                pin.SetinRouteIconColor();
            }
            foreach (var pin in Route.RoutePins.Where(x => x.Oid != Guid.Empty))
            {
                pin.SetIconFromRouteRow();
            }
            if (SelectedPin != null && SelectedPin.Pin != null)
            {
                SelectedPin.SetSelectedIconColor();
            }
        }

        private async Task SearchOnList(ObservableCollection<CustomPin> listViewSource, string filter, ExtendedListView listView)
        {
            await Task.Delay(1);
            listViewSource = new ObservableCollection<CustomPin>(AllPinsList.Where(x =>
                                                               !SelectedPins.Contains(x) &&
                                                               (x.AddressDescription.ToUpper().Contains(filter.ToUpper()) ||
                                                               x.CustomerDescription.ToUpper().Contains(filter.ToUpper()))));
            listView.ItemsSource = listViewSource;
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblAvailiableAddresses.Text = ResourcesRest.AvailiableAddresses;
            lblSelectedAddresses.Text = ResourcesRest.SelectedAddresses;
            btnCreateRoute.Text = ResourcesRest.CreateRoute;
            //btnSaveRoute.Text = ResourcesRest.SaveRoute;
            //btnInsertMapAddresses.Text = ResourcesRest.InsertMapAddresses;
            btnSaveName.Text = ResourcesRest.Save;
            btnCancelName.Text = ResourcesRest.Cancel;
            lblSaveName.Text = ResourcesRest.RouteName;
            btnCreateRoute.Clicked += (s, e) =>
            {
                var a = 1;
            };

        }

    }
}
