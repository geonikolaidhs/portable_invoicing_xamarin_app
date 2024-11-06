using System;
using System.Collections.Generic;
using System.Globalization;
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
    public static class AppExtensions
    {

        public static bool TryParse(this String str, out double value)
        {
            CultureInfo info = CultureInfo.GetCultureInfo("el-GR");
            str = str.Replace(".", ",");
            if (double.TryParse(str, NumberStyles.Any, info, out value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool TryParse(this String str, out decimal value)
        {
            CultureInfo info = CultureInfo.GetCultureInfo("el-GR");
            str = str.Replace(".", ",");
            if (decimal.TryParse(str, NumberStyles.Any, info, out value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}