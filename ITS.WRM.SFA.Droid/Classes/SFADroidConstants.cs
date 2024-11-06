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
    public static class SFADroidConstants
    {
        public const string CONNECTION_STRING = "ITS_WRM_SFA.db";
        public const string SETTINGS_FILE = "SFA.xml";
        public const string SFA_ZIP_FILE = "/POS/SFADatabase.zip";
        public const string DESTINATION_FILE_NAME = "SFADatabase.zip";
        public const string ITS_SYNC_FOLDER = "ITS";
    }

}