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
using Xamarin.Essentials;

namespace ITS.WRM.SFA.Droid.Classes.Devices
{
    public static class Screen
    {
        public static DisplayInfo DisplayInfo
        {
            get
            {
                return DeviceDisplay.MainDisplayInfo;
            }
        }

        public static DisplayOrientation Orientation
        {
            get
            {
                return DeviceDisplay.MainDisplayInfo.Orientation;
            }
        }
        public static DisplayRotation Rotation
        {
            get
            {
                return DeviceDisplay.MainDisplayInfo.Rotation;
            }
        }
        public static double Width
        {
            get
            {
                return DeviceDisplay.MainDisplayInfo.Width;
            }
        }
        public static double Height
        {
            get
            {
                return DeviceDisplay.MainDisplayInfo.Height;
            }
        }

        public static double Density
        {
            get
            {
                return DeviceDisplay.MainDisplayInfo.Density;
            }
        }
    }
}