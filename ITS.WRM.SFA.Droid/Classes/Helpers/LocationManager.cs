using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;
using GeoCoordinatePortable;
using ITS.WRM.SFA.Model.Model.NonPersistant;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public static class LocationManager
    {

        private static Geocoder Geocoder = new Geocoder();
        public static async Task<AddressLocation> GetCurrentlocation()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            AddressLocation addressLocation = null;
            try
            {
                Xamarin.Essentials.Location location = null;

                var request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(10));
                request.Timeout = TimeSpan.FromSeconds(10);
                location = await Geolocation.GetLocationAsync(request);
                if (location != null)
                {
                    addressLocation = new AddressLocation();
                    addressLocation.Longitude = location.Longitude;
                    addressLocation.Latitude = location.Latitude;
                }
                return addressLocation;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return addressLocation;
            }
            finally
            {
                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
            }
        }


        public static async Task<IEnumerable<string>> GetAddressesForPositionAsync(Position pos)
        {
            return await Geocoder.GetAddressesForPositionAsync(pos);
        }

        public static async Task<IEnumerable<Position>> GetPositionsForAddressAsync(string addr)
        {
            return await Geocoder.GetPositionsForAddressAsync(addr);
        }

        private static string FormatLocation(Location location, Exception ex = null)
        {
            string notAvailable = "not available";
            if (location == null)
            {
                return $"Unable to detect location. Exception: {ex?.Message ?? string.Empty}";
            }

            return
                $"Latitude: {location.Latitude}\n" +
                $"Longitude: {location.Longitude}\n" +
                $"Accuracy: {location.Accuracy}\n" +
                $"Altitude: {(location.Altitude.HasValue ? location.Altitude.Value.ToString() : notAvailable)}\n" +
                $"Heading: {(location.Course.HasValue ? location.Course.Value.ToString() : notAvailable)}\n" +
                $"Speed: {(location.Speed.HasValue ? location.Speed.Value.ToString() : notAvailable)}\n" +
                $"Date (UTC): {location.Timestamp:d}\n" +
                $"Time (UTC): {location.Timestamp:T}" +
                $"Moking Provider: {location.IsFromMockProvider}";
        }


        public static CustomPin GetClosestDistance(Position sourcePosition, List<CustomPin> destinations)
        {

            var sCoord = new GeoCoordinate(sourcePosition.Latitude, sourcePosition.Longitude);
            CustomPin closestPin = null;
            double minDistance = 0;
            int index = 0;
            foreach (var pin in destinations)
            {
                var eCoord = new GeoCoordinate(pin.Latitude, pin.Longitude);
                var distance = sCoord.GetDistanceTo(eCoord);
                if (index == 0)
                {
                    closestPin = pin;
                    minDistance = distance;
                }
                else if (distance < minDistance)
                {
                    closestPin = pin;
                    distance = minDistance;
                }
                index++;
            }
            return closestPin;
        }

    }

    public class AddressLocation
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }



}
