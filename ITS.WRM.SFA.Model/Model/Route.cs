using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.GoogleMaps;

namespace ITS.WRM.SFA.Model.Model
{
    public class Route : BasicObj
    {

        public Route()
        {
            RouteSteps = new List<RouteStep>();
            RoutePolyLine = new Polyline();
        }
        public string Name { get; set; }

        [Ignore]
        public int NumberOfPlaces
        {
            get
            {
                return RoutePins.Count;
            }
        }

        public Polyline RoutePolyLine = null;

        public string SerializedPins { get; set; }

        [Ignore]
        public double TotalTime
        {
            get
            {
                double time = 0;
                RoutePins?.ForEach(x => x?.DirectionsApiResponse?.routes[0]?.legs[0]?.steps?.ForEach(z => time += z?.duration?.value ?? 0));
                return time;
            }
        }

        [Ignore]
        public double TotalDistance
        {
            get
            {
                double distance = 0;
                RoutePins?.ForEach(x => x?.DirectionsApiResponse?.routes[0]?.legs[0]?.steps?.ForEach(z => distance += z?.distance?.value ?? 0));
                return distance;
            }
        }

        [Ignore]
        public List<CustomPin> RoutePins
        {
            get
            {
                if (_RoutePins == null && !string.IsNullOrEmpty(SerializedPins))
                {
                    _RoutePins = JsonConvert.DeserializeObject<List<CustomPin>>(SerializedPins);
                }
                if (_RoutePins == null)
                {
                    _RoutePins = new List<CustomPin>();
                }
                return _RoutePins;
            }
            set
            {
                _RoutePins = value;
                SerializedPins = JsonConvert.SerializeObject(_RoutePins);

            }
        }

        [Ignore]
        public Position StartPosition
        {
            get
            {
                return new Position(StartLat, StartLot);
            }
        }

        public double StartLat { get; set; }

        public double StartLot { get; set; }

        [Ignore]
        private List<CustomPin> _RoutePins { get; set; }



        [Ignore]
        public List<RouteStep> RouteSteps { get; set; }


    }
}
