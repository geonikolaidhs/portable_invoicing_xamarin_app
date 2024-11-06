using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using ITS.WRM.SFA.Droid.Classes;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.CurrentActivity;
using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

namespace ITS.WRM.SFA.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.DarkActionBar", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity, ILocationListener
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                ServicePointManager
                .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                Xamarin.Forms.Forms.Init(this, savedInstanceState);
                global::Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
                AndroidMethods objAndroidMethods = new AndroidMethods();
                objAndroidMethods.GenerateDatabase();
                CrossCurrentActivity.Current.Init(this, savedInstanceState);
                LoadApplication(new App());
                AppCenter.Start(App.AppCenterId, typeof(Analytics), typeof(Crashes));
                AppCenter.LogLevel = LogLevel.Error;
                UserDialogs.Init(this);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        public override void OnBackPressed()
        {
            if (!BackButtonManager.Instance.NotifyBackButtonPressed())
            {
                return;
            }
            base.OnBackPressed();
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == 16908332)
            {
                if (!BackButtonManager.Instance.NotifyBackButtonPressed())
                {
                    return base.OnOptionsItemSelected(item);
                }
                return base.OnOptionsItemSelected(item);
            }
            else
            {
                return base.OnOptionsItemSelected(item);
            }
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                App.LogError(exception);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }




        public void OnLocationChanged(Location location)
        {
            App.CurrentLocation = location;
            App.LogInfo("New Loaction : " + location.Longitude.ToString() + location.Latitude.ToString());
            // Called when a new location is found by the network location provider.
            //  loc = location;
        }

        public void OnProviderDisabled(string provider)
        {
            // throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //  throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

        protected override void OnPause()
        {
            base.OnPause();

        }

    }
}
