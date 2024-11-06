using ITS.WRM.SFA.Model.CustomEvents;
using ITS.WRM.SFA.Model.Helpers;
using System;
using System.Timers;
using Xamarin.Forms.GoogleMaps;


namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class CustomPin
    {
        public event EventHandler<CustomPinLongClickedEventArgs> PinLongClicked;

        public CustomPin(Guid oid, Pin pin)
        {
            Oid = oid;
            Pin = pin;
        }

        public CustomPin(Guid oid, Pin pin, Address address, Customer customer, Trader trader)
        {
            Oid = oid;
            Pin = pin;
            Address = address;
            Customer = customer;
            Trader = trader;
            TapTimer = new Timer();
            TapTimer.Interval = 2000;
            TapTimer.Enabled = false;
            TapTimer.Elapsed += Tapped;

            Pin.Clicked += (s, e) =>
           {
               lock (Pin)
               {
                   TapCounter++;
                   if (TapCounter == 1)
                   {
                       if (!TapTimer.Enabled)
                       {
                           TapTimer.Enabled = true;
                           TapTimer.Start();
                       }
                   }
                   if (TapCounter == 2)
                   {
                       SendPinLongClicked(this);
                       TapCounter = 0;
                       TapTimer.Stop();
                       TapTimer.Enabled = false;
                   }
               }
           };
        }

        public Guid Oid { get; set; }

        public Pin Pin { get; set; }

        public Address Address { get; set; }

        public Customer Customer { get; set; }

        public Trader Trader { get; set; }

        public metric Distance { get; set; }

        public metric Duration { get; set; }

        public DirectionsApiResponse DirectionsApiResponse { get; set; }

        public int RowNumber { get; set; } = -1;

        public bool InRoute { get; set; } = false;

        public bool InSelectedAddresses { get; set; } = false;

        private int TapCounter { get; set; }

        private void Tapped(object sender, EventArgs e)
        {
            TapCounter = 0;
            TapTimer.Enabled = false;
            TapTimer.Stop();
        }

        private Timer TapTimer = null;
        public string AddressDescription
        {
            get
            {
                return Address?.FullDescription;
            }
        }

        public string CustomerDescription
        {
            get
            {
                return Customer?.CompanyName + "  ( " + Customer.Code + " )";
            }
        }

        public double Longitude
        {
            get
            {
                return Address?.Longitude ?? 0;
            }
        }

        public double Latitude
        {
            get
            {
                return Address?.Latitude ?? 0;
            }
        }

        internal void SendPinLongClicked(CustomPin pin)
        {
            PinLongClicked?.Invoke(this, new CustomPinLongClickedEventArgs(pin));
        }

        public void SetSelectedIconColor()
        {
            this.Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.DefaultMarker(Xamarin.Forms.Color.Yellow);
        }

        public void SetinRouteIconColor()
        {
            this.Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.DefaultMarker(Xamarin.Forms.Color.Green);
        }


        public void SetDefaultIconColor()
        {
            this.Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.DefaultMarker(Xamarin.Forms.Color.Red);
        }

        public void SetIconFromRouteRow()
        {
            try
            {
                var view = new RoutePinView(RowNumber.ToString() ?? "", "White", "Transparent", "pin64.png");
                this.Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.FromView(view, Xamarin.Forms.Color.Green.GetHashCode().ToString());

            }
            catch (Exception ex)
            {
                var r = ex.Message;
            }
        }


        public void ClearFromRoute()
        {
            InRoute = false;
            Distance = new metric();
            Duration = new metric();
            RowNumber = -1;
            if (InSelectedAddresses)
            {
                this.Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.DefaultMarker(Xamarin.Forms.Color.Green);
            }
            else
            {
                this.Pin.Icon = Xamarin.Forms.GoogleMaps.BitmapDescriptorFactory.DefaultMarker(Xamarin.Forms.Color.Red);
            }
        }

    }
}
