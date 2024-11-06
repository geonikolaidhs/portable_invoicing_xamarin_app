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

namespace ITS.WRM.SFA.Droid.Classes
{
    public static class EventNames
    {
        public const string SCAN_EVENT = "ON.BARCODE.SCAN";

        public const string UPDATE_CURRENT_DOCUMENT_EVENT = "ON.UPDATE.CURRENT.DOCUMENT";

        public const string UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT = "ON.UPDATE.CURRENT.DOCUMENT.DETAILS";


    }
}