using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;
using LocationManager = ITS.WRM.SFA.Droid.Classes.Helpers.LocationManager;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCustomerAddress : ContentPage
    {
        private Address _CurrentAddress = null;
        private Customer _CurrentCustomer = null;
        private List<AddressType> ListAddressType = new List<AddressType>();
        private List<VatLevel> ListVatLevel = new List<VatLevel>();
        private static List<char> Numeric = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        Map _Map = null;
        Pin _Pin = null;

        public EditCustomerAddress(ref Address addr, Customer cust)
        {
            InitializeComponent();
            _CurrentAddress = addr;
            _CurrentCustomer = cust;
            InitiallizeControllers();
            InitialiseSelectOptions();
            if (_CurrentAddress == null)
            {
                throw new Exception("Address Not Found");
            }
            else
            {
                int index = 0;
                foreach (AddressType addressType in ListAddressType)
                {
                    if (_CurrentAddress != null && _CurrentAddress.AddressTypeOid == addressType.Oid)
                    {
                        pckAddressType.SelectedIndex = index;
                    }
                    index++;
                }
                index = 0;
                foreach (VatLevel vatlevel in ListVatLevel)
                {
                    if (!pckVatLevel.Items.Contains(vatlevel.Description))
                    {
                        pckVatLevel.Items.Add(vatlevel.Description);
                    }
                    if (_CurrentAddress != null && _CurrentAddress.VatLevelOid == vatlevel.Oid)
                    {
                        pckVatLevel.SelectedIndex = index;
                    }
                    index++;
                }

                StreetAddress.Text = _CurrentAddress.Street;
                NewCity.Text = _CurrentAddress.City;
                NewPostCode.Text = _CurrentAddress.PostCode;
                Coords.Text = _CurrentAddress.Longitude + " , " + _CurrentAddress.Latitude;
                if (_CurrentAddress.Longitude != 0 && _CurrentAddress.Latitude != 0)
                {
                    SetMapLocation(new Position(_CurrentAddress.Latitude, _CurrentAddress.Longitude));
                }
            }

            pckAddressType.SelectedIndexChanged += (sender, args) =>
            {
                _CurrentAddress.AddressTypeOid = pckAddressType.SelectedIndex == -1 ? Guid.Empty :
                                                              ListAddressType.Find(x => x.Description.Equals(pckAddressType.Items[pckAddressType.SelectedIndex]))?.Oid ?? Guid.Empty;
            };
            pckVatLevel.SelectedIndexChanged += (sender, args) =>
            {
                _CurrentAddress.VatLevelOid = pckVatLevel.SelectedIndex == -1 ? Guid.Empty :
                                                              ListVatLevel.Find(x => x.Description.Equals(pckVatLevel.Items[pckVatLevel.SelectedIndex]))?.Oid ?? Guid.Empty;
            };
        }

        private void InitialiseSelectOptions()
        {
            ListAddressType = App.DbLayer.GetAddressTypes();
            ListVatLevel = App.VatLevels;
            foreach (AddressType addressType in ListAddressType)
            {
                if (!pckAddressType.Items.Contains(addressType.Description))
                {
                    pckAddressType.Items.Add(addressType.Description);
                }
            }
            foreach (VatLevel vatlevel in ListVatLevel)
            {
                if (!pckVatLevel.Items.Contains(vatlevel.Description))
                {
                    pckVatLevel.Items.Add(vatlevel.Description);
                }
            }
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


        static void CalculateBoundingCoordinates(MapSpan region)
        {
            var center = region.Center;
            var halfheightDegrees = region.LatitudeDegrees / 2;
            var halfwidthDegrees = region.LongitudeDegrees / 2;

            var left = center.Longitude - halfwidthDegrees;
            var right = center.Longitude + halfwidthDegrees;
            var top = center.Latitude + halfheightDegrees;
            var bottom = center.Latitude - halfheightDegrees;


            if (left < -180) left = 180 + (180 + left);
            if (right > 180) right = (right - 180) - 180;


            Debug.WriteLine("Bounding box:");
            Debug.WriteLine("                    " + top);
            Debug.WriteLine("  " + left + "                " + right);
            Debug.WriteLine("                    " + bottom);
        }

        async void SetLocation(object sender, EventArgs eventArgs)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                if (string.IsNullOrEmpty(StreetAddress.Text) || string.IsNullOrEmpty(NewCity.Text))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.PleaseFillInAllRequiredFields, ResourcesRest.MsgBtnOk);
                        return;
                    });
                    UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                    return;
                }
                var list = await LocationManager.GetPositionsForAddressAsync(StreetAddress.Text + ", " + NewCity.Text + " " + NewPostCode.Text);
                Position pos = new Position();
                if (list != null)
                {
                    pos = list.FirstOrDefault();
                }
                SetMapLocation(pos);
                if (pos != null)
                {
                    Coords.Text = pos.Longitude + " , " + pos.Latitude;
                }
                UserDialogs.Instance.HideLoading();
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

        private void SetMapLocation(Position pos)
        {
            if (pos != null)
            {
                Coords.Text = pos.Longitude + " , " + pos.Latitude;
                _CurrentAddress.Longitude = pos.Longitude;
                _CurrentAddress.Latitude = pos.Latitude;

                _Map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(50)));
                if (_Map.Pins != null && _Map.Pins.Count > 0)
                {
                    IList<Pin> mapPins = new List<Pin>();
                    foreach (Pin p in _Map.Pins)
                    {
                        mapPins.Add(p);
                    }
                    foreach (Pin p in mapPins)
                    {
                        _Map.Pins.Remove(p);
                    }
                }
                _Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = pos,
                    Label = NewCity.Text,
                    Address = StreetAddress.Text
                };
                _Map.Pins.Add(_Pin);
                _Pin.IsDraggable = true;
            }
        }


        async void SaveAddress(object sender, EventArgs eventArguments)
        {
            try
            {
                if (string.IsNullOrEmpty(StreetAddress.Text) || _CurrentAddress.AddressTypeOid == null || _CurrentAddress.AddressTypeOid == Guid.Empty
          || _CurrentAddress.VatLevelOid == null || _CurrentAddress.VatLevelOid == Guid.Empty)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.PleaseFillInAllRequiredFields, ResourcesRest.MsgBtnOk);
                    return;
                }

                UserDialogs.Instance.ShowLoading(ResourcesRest.Save, MaskType.Black); UserDialogs.Instance.ShowLoading();

                if (_CurrentAddress != null)
                {
                    _CurrentAddress.Street = StreetAddress.Text;
                    _CurrentAddress.PostCode = NewPostCode.Text;
                    _CurrentAddress.City = NewCity.Text;
                    _CurrentAddress.VatLevel = ListVatLevel.Where(x => x.Oid == _CurrentAddress.VatLevelOid).FirstOrDefault();
                    _CurrentAddress.AddressType = ListAddressType.Where(x => x.Oid == _CurrentAddress.AddressTypeOid).FirstOrDefault();
                    _CurrentAddress.TraderOid = _CurrentCustomer.TraderOid;
                    _CurrentAddress.Trader = _CurrentCustomer.Trader;
                    _CurrentAddress.Profession = _CurrentAddress.Profession;

                    HttpResponseMessage response = await ApiHelper.EditAddress(_CurrentAddress);
                    if (response.IsSuccessStatusCode)
                    {
                        App.DbLayer.InsertOrReplaceObj<Address>(_CurrentAddress);
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.SuccessAdd, ResourcesRest.MsgBtnOk);

                    }
                    else
                    {
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.Fail, ResourcesRest.MsgBtnOk);
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }



        async void GetLocation(object sender, EventArgs eventArgs)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                AddressLocation Location = await LocationManager.GetCurrentlocation();
                if (Location == null)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, "Unable To Get Location", ResourcesRest.MsgBtnOk);
                }
                else
                {
                    try
                    {
                        if (Location != null)
                        {
                            Position pos = new Position(Location.Latitude, Location.Longitude);
                            string street = string.Empty;
                            string no = string.Empty;
                            string city = string.Empty;
                            string postCode = string.Empty;
                            string temp = string.Empty;
                            string address = string.Empty;
                            IEnumerable<string> possibleAddresses = await LocationManager.GetAddressesForPositionAsync(pos);
                            if (possibleAddresses != null)
                            {
                                List<string> list = possibleAddresses.ToList();
                                if (list.Count > 0)
                                {
                                    address = list.FirstOrDefault();
                                    string[] arr = address.Split(',');
                                    if (arr != null)
                                    {
                                        if (arr.Length > 0)
                                        {
                                            street = arr[0];
                                        }
                                        if (arr.Length > 1)
                                        {
                                            temp = arr[1];
                                            string[] split = temp.Trim(' ').Split(' ');
                                            if (split != null && split.Length > 0)
                                            {
                                                city = split[0].Trim(' ');
                                            }
                                            postCode = temp.Replace(city, " ").Trim(' ');
                                        }
                                    }
                                }
                            }
                            StreetAddress.Text = street;
                            NewCity.Text = city;
                            NewPostCode.Text = postCode;
                            SetMapLocation(pos);
                        }
                    }
                    catch (Exception ex)
                    {
                        App.LogError(ex);
                    }
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }




        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");

            btnSave.Text = ResourcesRest.Save;
            btnHideAddressForm.Text = ResourcesRest.Cancel;
            StreetAddress.Placeholder = ResourcesRest.Street;
            NewCity.Placeholder = ResourcesRest.City;
            NewPostCode.Placeholder = ResourcesRest.PostCode;
            pckAddressType.Title = ResourcesRest.AddressType;
            pckVatLevel.Title = ResourcesRest.VatLevel;
            lblStreetAddress.Text = ResourcesRest.Address;
            lblCity.Text = ResourcesRest.City;
            lblPostCode.Text = ResourcesRest.PostCode;
            lblVatLevel.Text = ResourcesRest.VatLevel;
            lblAddressType.Text = ResourcesRest.AddressType;
            lblCoords.Text = ResourcesRest.Coordinates;
            btnGetLocation.Text = ResourcesRest.GetLocation;
            btnHideAddressForm.Text = ResourcesRest.Close;
            lblEditAddress.Text = ResourcesRest.EditAddress;
            btnSetLocation.Text = ResourcesRest.SetLocationFromAddress;

            if (!(ListAddressType.Count > 0))
            {
                AddressTypeStack.IsVisible = false;
                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.AddressType + " Not Found", ResourcesRest.MsgBtnOk);
            }

            AddressTypeStack.IsVisible = true;
            Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus> permissions = new Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus>();

            var check = Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
            var shouldAsk = Plugin.Permissions.CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Location);

            if (check.Result != Plugin.Permissions.Abstractions.PermissionStatus.Granted || shouldAsk.Result == true)
            {
                permissions = Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location).Result;
            }
            else
            {
                permissions.Add(Plugin.Permissions.Abstractions.Permission.Location, Plugin.Permissions.Abstractions.PermissionStatus.Granted);
            }

            var checkResult = check.Result;
            bool value = permissions.TryGetValue(Plugin.Permissions.Abstractions.Permission.Location, out checkResult);
            if (value == false || check.Result != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + "Application Require Location Permissions", ResourcesRest.MsgBtnOk);
                    return;
                });
                return;
            }

            if (_Map == null)
            {
                _Map = new Map
                {
                    HasScrollEnabled = true,
                    HasZoomEnabled = true,
                    IsShowingUser = true,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    MapType = MapType.Hybrid
                };
                Map.Children.Add(_Map);
            }

            _Map.MapLongClicked += async (s, e) =>
            {
                try
                {
                    Position pos = e.Point;
                    await UpdateAddressLocation(pos);
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }
            };



            _Map.PinDragEnd += async (s, e) =>
            {
                try
                {
                    Position pos = e.Pin.Position;
                    await UpdateAddressLocation(pos);
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }

            };

            _Map.PropertyChanged += (s, e) =>
            {
                App.LogInfo(e.PropertyName);
                Debug.WriteLine(e.PropertyName + " just changed!");
                if (e.PropertyName.ToUpper().Contains("ZOOM"))
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, e.PropertyName, ResourcesRest.MsgBtnOk);
                }
                if (e.PropertyName == "VisibleRegion" && _Map.VisibleRegion != null)
                {
                    CalculateBoundingCoordinates(_Map.VisibleRegion);
                }
            };

            if (App.CurrentLocation != null)
            {
                SetMapLocation(new Position(App.CurrentLocation.Latitude, App.CurrentLocation.Longitude));
            }

        }


        private async Task UpdateAddressLocation(Position pos)
        {
            string street = string.Empty;
            string no = string.Empty;
            string city = string.Empty;
            string postCode = string.Empty;
            string temp = string.Empty;
            string address = string.Empty;
            IEnumerable<string> possibleAddresses = await LocationManager.GetAddressesForPositionAsync(pos);
            if (possibleAddresses != null)
            {
                List<string> list = possibleAddresses.ToList();
                if (list.Count > 0)
                {
                    address = list.FirstOrDefault();
                    string[] arr = address.Split(',');
                    if (arr != null)
                    {
                        if (arr.Length > 0)
                        {
                            street = arr[0];
                        }
                        if (arr.Length > 1)
                        {
                            temp = arr[1];
                            string[] split = temp.Trim(' ').Split(' ');
                            if (split != null && split.Length > 0)
                            {
                                city = split[0].Trim(' ');
                            }
                            postCode = temp.Replace(city, " ").Trim(' ');
                        }
                    }
                }
            }
            StreetAddress.Text = street;
            NewCity.Text = city;
            NewPostCode.Text = postCode;
            SetMapLocation(pos);
        }

        async void HideAddressForm(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }


        public void StreetAddressUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void StreetAddressFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        public void NewCityUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void NewCityFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        public void PostcodeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void PostcodeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

    }
}
