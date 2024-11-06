using Android.Content;
using Android.Locations;
using Android.Net;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.DocumentFormat;
using ITS.WRM.SFA.Droid.Pages;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Droid
{
    public partial class App : Application
    {
        public static SFASettings SFASettings { get; set; }
        public static Token token { get; internal set; }
        public static bool Login { get; internal set; }
        public static Guid UserId { get; set; }
        public static string UserName { get; set; }
        public static string UserPassword { get; set; }
        private static Store _Store { get; set; } = null;
        private static List<Store> _Stores { get; set; } = null;
        private static CompanyNew _Owner { get; set; } = null;
        private static OwnerApplicationSettings _OwnerApplicationSettings { get; set; } = null;
        private static ReceiptSchema _ReceiptSchema { get; set; }
        private static List<ItemCategory> _AllCategories { get; set; } = null;
        private static VatCategory _DefaultVatCategory { get; set; } = null;
        private static List<VatLevel> _VatLevels { get; set; } = null;
        private static List<VatFactor> _VatFactors { get; set; } = null;
        private static List<VatCategory> _VatCategories { get; set; } = null;
        private static List<MeasurementUnit> _MeasurementUnits { get; set; } = null;
        private static List<StorePriceList> _StorePriceList { get; set; } = null;
        private static List<PriceCatalog> _PriceCatalogs { get; set; } = null;
        private static List<PriceCatalogPolicy> _PriceCatalogPolicies { get; set; } = null;
        private static List<PriceCatalogPolicyDetail> _PriceCatalogPolicyDetails { get; set; } = null;
        private static List<DocumentType> _DocumentTypes { get; set; } = null;
        private static List<DocumentStatus> _DocumentStatuses { get; set; } = null;
        private static List<DocumentSeries> _DocumentSeries { get; set; } = null;
        private static List<BarcodeType> _BarcodeTypes { get; set; } = null;

        private static Location _CurrentLocation { get; set; }

        public static bool SkipAsking = false;

#if DEBUG
        public static string AppCenterId = "468a53de-6537-4b67-bf61-5500cd65d2b3";
#else
        public static string AppCenterId = "c328e86e-f110-4f04-ab91-cf39ba0d5865";
#endif

        public static Location CurrentLocation
        {
            get
            {
                return _CurrentLocation;
            }
            set { _CurrentLocation = value; }
        }

        public static Store Store
        {
            get
            {
                if (_Store == null)
                {
                    _Store = App.DbLayer.GetById<Store>(App.SFASettings.DefaultStore);
                }
                return _Store;
            }
        }


        public static VatLevel StoreVatLevel
        {
            get
            {
                if (Store != null && Store.Address == null && Store.AddressOid != null && Store.AddressOid != Guid.Empty)
                {
                    Store.Address = App.DbLayer.GetById<ITS.WRM.SFA.Model.Model.Address>(Store.AddressOid);
                }
                return VatLevels.Where(x => x.Oid == Store.Address.VatLevelOid).FirstOrDefault();
            }
        }
        public static List<Store> Stores
        {
            get
            {
                if (_Stores == null)
                {
                    _Stores = App.DbLayer.GetAllStores();
                }
                return _Stores;
            }
        }
        public static CompanyNew Owner
        {
            get
            {
                if (_Owner == null)
                {
                    _Owner = App.DbLayer.GetById<CompanyNew>(App.Store.OwnerOid);
                }
                return _Owner;
            }
        }
        public static OwnerApplicationSettings OwnerApplicationSettings
        {
            get
            {
                if (_OwnerApplicationSettings == null)
                {
                    _OwnerApplicationSettings = App.DbLayer.GetOwnerApplicationSettings(App.Store.OwnerOid);
                }
                return _OwnerApplicationSettings;
            }
        }


        public static List<ItemCategory> AllCategories
        {
            get
            {
                if (_AllCategories == null)
                {
                    _AllCategories = App.DbLayer.GetAllItemCategories();
                    foreach (ITS.WRM.SFA.Model.Model.ItemCategory category in _AllCategories)
                    {
                        category.FullDescription = (category.ParentOid == null || category.ParentOid == Guid.Empty) ? category.FullDescription = "<- " + category.Description : "<- " + category.CategoryPath(App.DbLayer) + "->" + category.Description;
                    }
                }
                return _AllCategories;
            }
        }
        public static VatCategory DefaultVatCategory
        {
            get
            {
                if (_DefaultVatCategory == null)
                {
                    _DefaultVatCategory = App.DbLayer.GetDefaultVatCategoty();
                }
                return _DefaultVatCategory;
            }
        }
        public static List<VatLevel> VatLevels
        {
            get
            {
                if (_VatLevels == null)
                {
                    _VatLevels = App.DbLayer.GetVatLevels();
                }
                return _VatLevels;
            }
        }
        public static List<VatCategory> VatCategories
        {
            get
            {
                if (_VatCategories == null)
                {
                    _VatCategories = App.DbLayer.GetVatCategories();
                }
                return _VatCategories;
            }
        }
        public static List<VatFactor> VatFactors
        {
            get
            {
                if (_VatFactors == null)
                {
                    _VatFactors = App.DbLayer.GetVatFactors();
                }
                return _VatFactors;
            }
        }
        public static List<MeasurementUnit> MeasurementUnits
        {
            get
            {
                if (_MeasurementUnits == null)
                {
                    _MeasurementUnits = App.DbLayer.GetMeasurementUnits();
                }
                return _MeasurementUnits;
            }
        }
        public static List<StorePriceList> StorePriceList
        {
            get
            {
                if (_StorePriceList == null)
                {
                    _StorePriceList = App.DbLayer.GetStorepriceList(App.Store.Oid);
                }
                return _StorePriceList;
            }
        }
        public static List<PriceCatalog> StorePriceCatalogs
        {
            get
            {
                List<PriceCatalog> list = new List<PriceCatalog>();
                foreach (StorePriceList spl in App.StorePriceList)
                {
                    foreach (PriceCatalog catalog in App.PriceCatalogs)
                    {
                        if (spl.PriceCatalogOid == catalog.Oid)
                        {
                            list.Add(catalog);
                            break;
                        }
                    }
                }
                return list;
            }
        }
        public static List<PriceCatalog> PriceCatalogs
        {
            get
            {
                if (_PriceCatalogs == null)
                {
                    _PriceCatalogs = App.DbLayer.GetAllPriceCatalogs();
                }
                return _PriceCatalogs;
            }
        }
        public static List<PriceCatalogPolicy> PriceCatalogPolicies
        {
            get
            {
                if (_PriceCatalogPolicies == null)
                {
                    _PriceCatalogPolicies = App.DbLayer.GetAllPriceCatalogPolicies();
                }
                return _PriceCatalogPolicies;
            }
        }
        public static List<PriceCatalogPolicyDetail> PriceCatalogPolicyDetails
        {
            get
            {
                if (_PriceCatalogPolicyDetails == null)
                {
                    _PriceCatalogPolicyDetails = App.DbLayer.GetAllPriceCatalogPolicyDetails();
                }
                return _PriceCatalogPolicyDetails;
            }
        }
        public static List<DocumentType> DocumentTypes
        {
            get
            {
                if (_DocumentTypes == null)
                {
                    _DocumentTypes = App.DbLayer.GetAllDocumentTypes();
                }
                return _DocumentTypes;
            }
        }
        public static List<DocumentSeries> DocumentSeries
        {
            get
            {
                if (_DocumentSeries == null)
                {
                    _DocumentSeries = App.DbLayer.GetAllDocumentSeries();
                }
                return _DocumentSeries;
            }
        }
        public static List<DocumentStatus> DocumentStatuses
        {
            get
            {
                if (_DocumentStatuses == null)
                {
                    _DocumentStatuses = App.DbLayer.GetAllDocumentStatus();
                }
                return _DocumentStatuses;
            }
        }

        public static List<BarcodeType> BarcodeTypes
        {
            get
            {
                if (_BarcodeTypes == null)
                {
                    _BarcodeTypes = App.DbLayer.GetAllBarcodeTypes();
                }
                return _BarcodeTypes;
            }
        }

        private static DatabaseLayer _DBLayer { get; set; } = null;
        public static DatabaseLayer DbLayer
        {
            get
            {
                if (_DBLayer == null)
                {
                    _DBLayer = DependencyService.Get<ICrossPlatformMethods>().GetDataBaseLayer();
                }
                return _DBLayer;
            }
        }
        public App()
        {
            if (DependencyService.Get<ICrossPlatformMethods>().SettingsFileExists())
            {
                SFASettings = ReadSettingsFromXML();
                string error = string.Empty;
                App.SFASettings = SFASettings;
                Task.Run(() => _Refresh().ConfigureAwait(false));
                if (!DependencyService.Get<ICrossPlatformMethods>().ValidateSettings(SFASettings, out error))
                {
                    MainPage = new NavigationPage(new SettingsTabPage("Login"));
                }
                else if (App.Login)
                {
                    MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    MainPage = new NavigationPage(new LoginPage());
                }
            }
            else
            {
                SFASettings = new SFASettings();
                MainPage = new NavigationPage(new SettingsTabPage("Login"));
            }
        }
        public static async Task Refresh()
        {
            await _Refresh().ConfigureAwait(false);
        }
        private static async Task _Refresh()
        {

            _Store = null;
            _ReceiptSchema = null;
            _Stores = null;
            _Owner = null;
            _OwnerApplicationSettings = null;
            _AllCategories = null;
            _DefaultVatCategory = null;
            _VatLevels = null;
            _VatCategories = null;
            _VatFactors = null;
            _MeasurementUnits = null;
            _StorePriceList = null;
            _PriceCatalogs = null;
            _PriceCatalogPolicies = null;
            _PriceCatalogPolicyDetails = null;
            _DocumentTypes = null;
            _DocumentSeries = null;
            _DocumentStatuses = null;
            _BarcodeTypes = null;

            var Store = App.Store;
            var Stores = App.Stores;
            var Owner = App.Owner;
            var OwnerApplcationSeittings = App.OwnerApplicationSettings;
            var AllCategories = App.AllCategories;
            var DefaultVatCategory = App.DefaultVatCategory;
            var VatLevels = App.VatLevels;
            var VatCategories = App.VatCategories;
            var VatFactors = App.VatFactors;
            var MeasurementUnits = App.MeasurementUnits;
            var StorePriceList = App.StorePriceList;
            var PriceCatalogs = App.PriceCatalogs;
            var PriceCatalogPolicies = App.PriceCatalogPolicies;
            var PriceCatalogPolicyDetails = App.PriceCatalogPolicyDetails;
            var DocumentTypes = App.DocumentTypes;
            var DocumentSeries = App.DocumentSeries;
            var DosumentStatuses = App.DocumentStatuses;
            var BarcodeTypes = App.BarcodeTypes;


            await Task.Delay(10);
        }
        public SFASettings ReadSettingsFromXML()
        {
            return DependencyService.Get<ICrossPlatformMethods>().ReadSettingsFromXML();
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static void LogError(Exception ex)
        {
            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Exception StackTrace : " + ex.StackTrace + (ex.InnerException != null ? " , InnerException : " + ex.InnerException.Message : ""));
        }

        public static void LogInfo(string info)
        {

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("Info : " + info);
        }

        public static CultureInfo Languageinfo(string LanguageId)
        {
            return System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(LanguageId);
        }

        public static NetworkInfo GetNetworkInfo()
        {
            try
            {
                Android.Net.ConnectivityManager connectivityManager = (Android.Net.ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
                NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
                return networkInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool IsNetworkAvailable()
        {
            return XLabs.Platform.Services.Reachability.IsNetworkAvailable();
        }


    }
}
