using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Xamarin.Forms.GoogleMaps;

namespace ITS.WRM.SFA.Model.CustomEvents
{
    public class CustomPinLongClickedEventArgs : EventArgs
    {


        public CustomPin CustomPin { get; }


        internal CustomPinLongClickedEventArgs(CustomPin customPin)
        {
            this.CustomPin = customPin;
        }
    }
}