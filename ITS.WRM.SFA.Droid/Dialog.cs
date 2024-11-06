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

namespace ITS.WRM.SFA.Droid
{
    [Activity(Label = "Dialog")]
    public class Dialog : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);

#pragma warning disable CS0618 // Type or member is obsolete
            ProgressDialog progress = new ProgressDialog(this);
#pragma warning restore CS0618 // Type or member is obsolete
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Contacting server. Please wait...");
            progress.SetCancelable(false);
            progress.Show();
        }
    }

}