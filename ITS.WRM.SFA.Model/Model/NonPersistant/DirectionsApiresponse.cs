using System.Collections.Generic;
using Xamarin.Forms.GoogleMaps;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class DirectionsApiResponse
    {
        public DirectionsApiResponse()
        {
            routes = new List<route>();
        }
        public List<route> routes { get; set; }
    }

    public class route
    {
        public route()
        {
            legs = new List<leg>();
        }

        public List<leg> legs { get; set; }

        public polyline overview_polyline { get; set; }
    }

    public class leg
    {
        public leg()
        {
            steps = new List<step>();
        }

        public metric distance { get; set; }
        public metric duration { get; set; }

        public List<step> steps { get; set; }
    }

    public class step
    {
        public metric distance { get; set; }
        public metric duration { get; set; }

        public location end_location { get; set; }
        public location start_location { get; set; }

        public polyline polyline { get; set; }
    }

    public class location
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class polyline
    {
        public string points { get; set; }
    }



    public class metric
    {
        public string text { get; set; }
        public double value { get; set; }
    }


    public class RouteStep
    {
        public RouteStep(int number, Position position, string polyline)
        {
            this.Position = position;
            this.Number = number;
            this.Polyline = polyline;
        }

        public int Number { get; set; }

        public Position Position { get; set; }

        public string Polyline { get; set; }
    }




}
