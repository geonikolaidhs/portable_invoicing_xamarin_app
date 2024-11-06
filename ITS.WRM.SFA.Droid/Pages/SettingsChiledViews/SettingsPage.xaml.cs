using Acr.UserDialogs;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Widget;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Devices;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace ITS.WRM.SFA.Droid.Pages.SettingsChiledViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private List<Encoding> Encodings = new List<Encoding>() { Encoding.ASCII, Encoding.BigEndianUnicode, Encoding.Unicode, Encoding.UTF7, Encoding.UTF8, Encoding.GetEncoding(1253), Encoding.GetEncoding(437)
    };
        private List<ItemCategory> ItemCategories = new List<ItemCategory>();
        private List<Language> ListLanguages = new List<Language>() { new Language() { Nationality = "en-GB", Description = "English" }, new Language() { Nationality = "el-GR", Description = "Ελληνικά" } };
        private List<DocumentStatus> ListDocumentStatus = new List<DocumentStatus>();
        private List<DocumentType> AllDocumentTypes = new List<DocumentType>();
        private List<DocumentType> SfaDocumentTypes = new List<DocumentType>();
        private List<DocumentType> LoadingDocumentTypes = new List<DocumentType>();
        private List<DocumentType> UnLoadingDocumentTypes = new List<DocumentType>();
        List<StoreDocumentSeriesType> StoreDocumentSeriesTypeList = new List<StoreDocumentSeriesType>();
        private List<Store> Stores = new List<Store>();
        private List<DocumentSeries> DocumentSeries = new List<DocumentSeries>();
        private List<string> ListOfDevices { get; set; } = new List<string>();
        private List<int> ListOfTablets { get; set; } = new List<int>();
        private bool _IsConnected = false;
        private Guid SelectedCategoryOid;
        private Guid SelectedStoreOid;
        private Guid SelectedSeriesOid;
        private Guid SelectedDocumentTypeOid;
        private Guid SelectedDocumentStatusOid;
        private Guid SelectedDocumentStatusToSendOid;
        private Guid SelectedLoadingDocumentTypeOid;
        private Guid SelectedUnLoadingDocumentTypeOid;
        private Guid SelectedUnLoadingSeriesOid;
        private string SelectedLanguage;
        private string SelectedBlueToothPrinter;
        private string SelectedBlueToothScanner;
        private int SelectedTabletId;
        private string strOk;
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        public string MsgSaveSettingsToContinue { get; private set; }
        private string _PageComeFrom;

        public SettingsPage(string PageComeFrom)
        {
            try
            {
                _PageComeFrom = PageComeFrom;
                InitializeComponent();
                InitiallizeControllers();
                InitialiseOptions();
                #region 
                /// DOCUMENT TYPE CHANGE EVENTS///
                pckDocumentType.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckDocumentType.SelectedIndex == -1)
                    {
                        SelectedDocumentTypeOid = Guid.Empty;
                    }
                    else
                    {
                        string DocumentTypeDescription = pckDocumentType.Items[pckDocumentType.SelectedIndex];
                        SelectedDocumentTypeOid = SfaDocumentTypes.Where(x => x.Description == DocumentTypeDescription).FirstOrDefault()?.Oid ?? Guid.Empty;
                        SelectedSeriesOid = Guid.Empty;
                        pckDocumentSeries.SelectedIndex = -1;
                    }
                    LoadDocumentSeries(SelectedDocumentTypeOid, SelectedSeriesOid);
                };
                pckLoadingDocumentType.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckLoadingDocumentType.SelectedIndex == -1)
                    {
                        SelectedLoadingDocumentTypeOid = Guid.Empty;
                    }
                    else
                    {
                        string DocumentTypeDescription = pckLoadingDocumentType.Items[pckLoadingDocumentType.SelectedIndex];
                        SelectedLoadingDocumentTypeOid = LoadingDocumentTypes.Where(x => x.Description == DocumentTypeDescription).FirstOrDefault()?.Oid ?? Guid.Empty;
                    }
                };
                pckUnLoadingDocumentType.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckUnLoadingDocumentType.SelectedIndex == -1)
                    {
                        SelectedUnLoadingDocumentTypeOid = Guid.Empty;
                    }
                    else
                    {
                        string DocumentTypeDescription = pckUnLoadingDocumentType.Items[pckUnLoadingDocumentType.SelectedIndex];
                        SelectedUnLoadingDocumentTypeOid = SfaDocumentTypes.Where(x => x.Description == DocumentTypeDescription).FirstOrDefault()?.Oid ?? Guid.Empty;
                        SelectedUnLoadingSeriesOid = Guid.Empty;
                        pckUnLoadingDocumentSeries.SelectedIndex = -1;
                    }
                    UpdateUnLoadingDocumentSeries(SelectedUnLoadingDocumentTypeOid);
                };
                #endregion
                /// END DOCUMENT TYPE CHANGE EVENTS ///

                #region 
                /// SERIES CHANGE EVENTS ///
                pckDocumentSeries.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckDocumentSeries.SelectedIndex == -1)
                    {
                        SelectedSeriesOid = Guid.Empty;
                    }
                    else
                    {
                        string SeriesName = pckDocumentSeries.Items[pckDocumentSeries.SelectedIndex];
                        SelectedSeriesOid = DocumentSeries.Where(xx => xx != null).ToList()?.Where(x => x.Description == SeriesName).FirstOrDefault()?.Oid ?? Guid.Empty;
                    }
                };
                pckUnLoadingDocumentSeries.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckUnLoadingDocumentSeries.SelectedIndex == -1)
                    {
                        SelectedUnLoadingSeriesOid = Guid.Empty;
                    }
                    else
                    {
                        string SeriesName = pckUnLoadingDocumentSeries.Items[pckUnLoadingDocumentSeries.SelectedIndex];
                        SelectedUnLoadingSeriesOid = DocumentSeries.Where(xx => xx != null).ToList()?.Where(x => x.Description == SeriesName).FirstOrDefault()?.Oid ?? Guid.Empty;
                    }
                };
                #endregion
                /// END SERIES CHANGE EVENTS ///

                pckCategoryNodeList.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckCategoryNodeList.SelectedIndex == -1)
                    {
                        SelectedCategoryOid = Guid.Empty;
                    }
                    else
                    {
                        string CategoryNodeDescription = pckCategoryNodeList.Items[pckCategoryNodeList.SelectedIndex];
                        SelectedCategoryOid = ItemCategories.Where(x => x.Description == CategoryNodeDescription).FirstOrDefault()?.Oid ?? Guid.Empty;
                    }
                };
                pckLanguageList.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckLanguageList.SelectedIndex >= 0)
                    {
                        string LanguageDescription = pckLanguageList.Items[pckLanguageList.SelectedIndex];
                        App.SFASettings.Language = ListLanguages.Find(x => x.Description.Equals(LanguageDescription)).Nationality;
                        SelectedLanguage = App.SFASettings.Language;
                    }
                };
                pckPairScannerDevices.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckPairScannerDevices.SelectedIndex >= 0)
                    {
                        string selectedDevice = pckPairScannerDevices.Items[pckPairScannerDevices.SelectedIndex];
                        App.SFASettings.BlueToothScanner = ListOfDevices.Find(x => x.Equals(selectedDevice));
                        SelectedBlueToothScanner = App.SFASettings.BlueToothScanner;
                    }
                };
                pckPairPrinterDevices.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckPairPrinterDevices.SelectedIndex >= 0)
                    {
                        string selectedDevice = pckPairPrinterDevices.Items[pckPairPrinterDevices.SelectedIndex];
                        App.SFASettings.BlueToothPrinter = ListOfDevices.Find(x => x.Equals(selectedDevice));
                        SelectedBlueToothPrinter = App.SFASettings.BlueToothPrinter;
                    }
                };
                pckDevices.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckDevices.SelectedIndex >= 0)
                    {
                        int selectedDevice;
                        int.TryParse(pckDevices.Items[pckDevices.SelectedIndex], out selectedDevice);
                        App.SFASettings.SfaId = ListOfTablets.Find(x => x.Equals(selectedDevice));
                        SelectedTabletId = App.SFASettings.SfaId;
                    }
                };
                pckEncodingFrom.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckEncodingFrom.SelectedIndex >= 0)
                    {
                        string selectedEncoding = pckEncodingFrom.Items[pckEncodingFrom.SelectedIndex];
                        App.SFASettings.EncodingFrom = Encodings.Where(x => x.EncodingName == selectedEncoding).FirstOrDefault();
                    }
                };
                pckEncodingTo.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckEncodingTo.SelectedIndex >= 0)
                    {
                        string selectedEncoding = pckEncodingTo.Items[pckEncodingTo.SelectedIndex];
                        App.SFASettings.EncodingTo = Encodings.Where(x => x.EncodingName == selectedEncoding).FirstOrDefault();
                    }
                };
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private void OnStoreChanged(object sender, EventArgs args)
        {
            Picker item = (Picker)sender;
            if (item.SelectedIndex == -1)
            {
                SelectedStoreOid = Guid.Empty;
            }
            else
            {
                string StoreName = item.Items[item.SelectedIndex];
                SelectedStoreOid = Stores.Where(x => x.Name == StoreName).FirstOrDefault()?.Oid ?? Guid.Empty;
            }
            StoreChanged();
        }

        private void OnSendDocumentStatusChanged(object sender, EventArgs args)
        {
            Picker item = (Picker)sender;
            if (item.SelectedIndex == -1)
            {
                SelectedDocumentStatusToSendOid = Guid.Empty;
            }
            else
            {
                string name = item.Items[item.SelectedIndex];
                SelectedDocumentStatusToSendOid = ListDocumentStatus.Where(x => x.Description == name).FirstOrDefault()?.Oid ?? Guid.Empty;
            }
        }

        private void OnDefaultDocumentStatusChanged(object sender, EventArgs args)
        {
            Picker item = (Picker)sender;
            if (item.SelectedIndex == -1)
            {
                SelectedDocumentStatusOid = Guid.Empty;
            }
            else
            {
                string name = item.Items[item.SelectedIndex];
                SelectedDocumentStatusOid = ListDocumentStatus.Where(x => x.Description == name).FirstOrDefault()?.Oid ?? Guid.Empty;
            }
        }

        /// <summary>
        /// Dev Function to be removed
        /// </summary>
        private void SetSomeValues()
        {

#if DEBUG
            //string defaultAuthenticationURL = @"http://appservices.westeurope.cloudapp.azure.com/wrmapi/Token";
            //string defaultServerURL = @"http://appservices.westeurope.cloudapp.azure.com/wrmapi/api";
            //string defaultDownloadURL = @"http://appservices.westeurope.cloudapp.azure.com/WRMDUAL";

            string defaultAuthenticationURL = @"http://192.168.1.9/wrmapi/Token";
            string defaultServerURL = @"http://192.168.1.9/wrmapi/api";
            string defaultDownloadURL = @"http://192.168.1.9/webclient";

            //string defaultAuthenticationURL = @"http://xkesidis.webretailmarket.gr/wrmapi/Token";
            //string defaultServerURL = @"http://xkesidis.webretailmarket.gr/wrmapi/api";
            //string defaultDownloadURL = @"http://xkesidis.webretailmarket.gr/RetailDual";
#else
            string defaultAuthenticationURL = @"http://xkesidis.webretailmarket.gr/wrmapi/Token";
            string defaultServerURL = @"http://xkesidis.webretailmarket.gr/wrmapi/api";
            string defaultDownloadURL = @"http://xkesidis.webretailmarket.gr/RetailDual";
#endif
            if (string.IsNullOrEmpty(App.SFASettings.AuthenticationURL))
            {
                entryAuthenticationURL.Text = defaultAuthenticationURL;
            }

            if (string.IsNullOrEmpty(App.SFASettings.ServerURL))
            {
                entryServerURL.Text = defaultServerURL;
            }

            if (string.IsNullOrEmpty(App.SFASettings.DatabaseDownloadURL))
            {
                entryDownloadURL.Text = defaultDownloadURL;
            }
        }

        private void InitialiseOptions(bool databaseReload = false)
        {
            try
            {
                AllDocumentTypes = App.DbLayer.GetAllDocumentTypes();
                StoreDocumentSeriesTypeList = App.DbLayer.GetAllStoreDocumentSeriesTypes(SelectedStoreOid);
                DocumentSeries = App.DbLayer.GetAllDocumentSeries().Where(x => !x.IsCancelingSeries).ToList();
                Stores = App.DbLayer.GetAllStores();
                SfaDocumentTypes = AllDocumentTypes.Where(x => StoreDocumentSeriesTypeList.Select(z => z.DocumentTypeOid).ToList().Contains(x.Oid)).ToList();

                LoadingDocumentTypes = AllDocumentTypes.Where(x => x.ItemStockAffectionOptions == ItemStockAffectionOptions.INITIALISES).ToList();
                UnLoadingDocumentTypes = AllDocumentTypes.Where(x => x.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS).ToList();
                ListDocumentStatus = App.DbLayer.GetAllDocumentStatus();
                ItemCategories = App.DbLayer.GetAllRootCategoryNodes();
                ListOfTablets = App.DbLayer.GetAllSfaDevices().Select(x => x.ID).ToList();
                chkPrinterConvertEncoding.Checked = App.SFASettings.PrinterConvertEncoding; entryVehicleNumber.Text = App.SFASettings.VehicleNumber;
                entryPrinterLineChars.Text = App.SFASettings.PrinterLineChars > 0 ? App.SFASettings.PrinterLineChars.ToString() : 80.ToString();
                if (!databaseReload)
                {
                    entryAuthenticationURL.Text = App.SFASettings.AuthenticationURL;
                    entryServerURL.Text = App.SFASettings.ServerURL;
                    entryDownloadURL.Text = App.SFASettings.DatabaseDownloadURL;
                }
                entryTimeout.Text = App.SFASettings.ApiTimeout.ToString();
                SelectedCategoryOid = App.SFASettings.CategoryNode;
                SelectedStoreOid = App.SFASettings.DefaultStore;
                SelectedDocumentStatusOid = App.SFASettings.DefaultDocumentStatusOid;
                SelectedDocumentStatusToSendOid = App.SFASettings.DocumentStatusToSendOid;
                SelectedDocumentTypeOid = App.SFASettings.DefaultDocumentTypeOid;
                SelectedSeriesOid = App.SFASettings.DocumentSeries;
                SelectedLoadingDocumentTypeOid = App.SFASettings.LoadingDocumentTypeOid;
                SelectedUnLoadingSeriesOid = App.SFASettings.UnLoadingSeriesOid;
                SelectedUnLoadingDocumentTypeOid = App.SFASettings.UnLoadingDocumentTypeOid;
                SelectedLanguage = App.SFASettings.Language;
                SelectedBlueToothScanner = App.SFASettings.BlueToothScanner;
                SelectedBlueToothPrinter = App.SFASettings.BlueToothPrinter;
                SelectedTabletId = App.SFASettings.SfaId;

                LoadLanguages();
                LoadSfaDocumentTypes();
                ZPLSwitch.Checked = App.SFASettings.Zpl;
                SetSomeValues();
                ///LOAD & SET SELECTED ITEMCATEGORY///
                int index = 0;
                ItemCategories.ForEach(x =>
                {
                    if (!pckCategoryNodeList.Items.Contains(x.Description)) { pckCategoryNodeList.Items.Add(x.Description); }
                    if (x.Oid == SelectedCategoryOid) { pckCategoryNodeList.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET SELECTED DEFAULT DOCUMENTSTATUS & DOCUMENTSTATUS TO SEND///
                index = 0;
                ListDocumentStatus.ForEach(x =>
                {
                    if (!pckDocumentStatus.Items.Contains(x.Description)) { pckDocumentStatus.Items.Add(x.Description); }
                    if (!pckDocumentStatusToSend.Items.Contains(x.Description)) { pckDocumentStatusToSend.Items.Add(x.Description); }
                    if (x.Oid == SelectedDocumentStatusOid) { pckDocumentStatus.SelectedIndex = index; }
                    if (x.Oid == SelectedDocumentStatusToSendOid) { pckDocumentStatusToSend.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET  DEFAULT STORE///
                index = 0;
                Stores.ForEach(x =>
                {
                    if (!pckSelectedStore.Items.Contains(x.Name)) { pckSelectedStore.Items.Add(x.Name); }
                    if (x.Oid == SelectedStoreOid) { pckSelectedStore.SelectedIndex = index; }
                    index++;
                    StoreDocumentSeriesTypeList = App.DbLayer.GetAllStoreDocumentSeriesTypes(SelectedStoreOid);
                });

                ///LOAD & SET  DOCUMENT TYPE///
                index = 0;
                SfaDocumentTypes.ForEach(x =>
                {
                    if (!pckDocumentType.Items.Contains(x.Description)) { pckDocumentType.Items.Add(x.Description); }
                    if (x.Oid == SelectedDocumentTypeOid) { pckDocumentType.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET LOADING DOCUMENT TYPE///
                index = 0;
                LoadingDocumentTypes.ForEach(x =>
                {
                    if (!pckLoadingDocumentType.Items.Contains(x.Description)) { pckLoadingDocumentType.Items.Add(x.Description); }
                    if (x.Oid == SelectedLoadingDocumentTypeOid) { pckLoadingDocumentType.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET UNLOADING DOCUMENT TYPE///
                index = 0;
                UnLoadingDocumentTypes.ForEach(x =>
                {
                    if (!pckUnLoadingDocumentType.Items.Contains(x.Description)) { pckUnLoadingDocumentType.Items.Add(x.Description); }
                    if (x.Oid == SelectedUnLoadingDocumentTypeOid) { pckUnLoadingDocumentType.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET SFA CODE///        
                index = 0;
                foreach (int code in ListOfTablets)
                {
                    if (!pckDevices.Items.Contains(code.ToString()))
                    {
                        pckDevices.Items.Add(code.ToString());
                    }
                    if (code == SelectedTabletId)
                    {
                        pckDevices.SelectedIndex = index;
                    }
                    index++;
                }


                ///LOAD & SET  DOCUMENT SERIES TYPE///
                index = 0;
                if (SelectedDocumentTypeOid != null && SelectedDocumentTypeOid != Guid.Empty)
                {
                    pckDocumentSeries.IsEnabled = true;
                    DocumentSeries.ForEach(x =>
                    {
                        if (!pckDocumentSeries.Items.Contains(x.Description)) { pckDocumentSeries.Items.Add(x.Description); }
                        if (x.Oid == SelectedSeriesOid) { pckDocumentSeries.SelectedIndex = index; }
                        index++;
                    });
                    LoadDocumentSeries(SelectedDocumentTypeOid, SelectedSeriesOid);
                }
                else
                {
                    pckDocumentSeries.IsEnabled = false;
                }

                ///LOAD & SET  UNLOADING DOCUMENT SERIES ///
                index = 0;
                if (SelectedUnLoadingDocumentTypeOid != null && SelectedUnLoadingDocumentTypeOid != Guid.Empty)
                {
                    pckUnLoadingDocumentSeries.IsEnabled = true;
                    DocumentSeries.ForEach(x =>
                    {
                        if (!pckUnLoadingDocumentSeries.Items.Contains(x.Description)) { pckUnLoadingDocumentSeries.Items.Add(x.Description); }
                        if (x.Oid == SelectedUnLoadingSeriesOid) { pckUnLoadingDocumentSeries.SelectedIndex = index; }
                        index++;
                    });
                    UpdateUnLoadingDocumentSeries(SelectedUnLoadingDocumentTypeOid);
                }
                else
                {
                    pckUnLoadingDocumentSeries.IsEnabled = false;
                }

                ///LOAD & SET BLUETOOTH DEVICES///
                try
                {
                    ListOfDevices = DependencyService.Get<IBlueTooth>()?.PairedDevices()?.ToList() ?? new List<string>();
                    index = 0;
                    foreach (string deviceName in ListOfDevices)
                    {
                        if (!pckPairScannerDevices.Items.Contains(deviceName))
                        {
                            pckPairScannerDevices.Items.Add(deviceName);
                        }
                        if (deviceName == SelectedBlueToothScanner)
                        {
                            pckPairScannerDevices.SelectedIndex = index;
                        }
                        index++;
                    }
                    LoadEncodingPickers();
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                }


                try
                {
                    index = 0;
                    foreach (string deviceName in ListOfDevices)
                    {
                        if (!pckPairPrinterDevices.Items.Contains(deviceName))
                        {
                            pckPairPrinterDevices.Items.Add(deviceName);
                        }
                        if (deviceName == SelectedBlueToothPrinter)
                        {
                            pckPairPrinterDevices.SelectedIndex = index;
                        }
                        index++;
                    }
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
            if (SelectedStoreOid == null || SelectedStoreOid == Guid.Empty)
            {
                pckDocumentType.IsEnabled = false;
                pckDocumentSeries.IsEnabled = false;
                pckLoadingDocumentType.IsEnabled = false;
                pckUnLoadingDocumentType.IsEnabled = false;
                pckUnLoadingDocumentSeries.IsEnabled = false;
            }
        }

        private void LoadEncodingPickers()
        {

            ///LOAD & SET  PRINTER ENCODING FROM LIST///
            int index = 0;
            if (App.SFASettings.PrinterConvertEncoding)
            {
                pckEncodingFrom.IsEnabled = true;
                Encodings.ForEach(x =>
                {
                    if (!pckEncodingFrom.Items.Contains(x.EncodingName)) { pckEncodingFrom.Items.Add(x.EncodingName); }
                    if (x.EncodingName == App.SFASettings.EncodingFrom?.EncodingName) { pckEncodingFrom.SelectedIndex = index; }
                    index++;
                });
            }
            else
            {
                pckEncodingFrom.IsEnabled = false;
                App.SFASettings.EncodingFrom = null;
            }


            ///LOAD & SET  PRINTER ENCODING TO LIST///
            index = 0;
            if (App.SFASettings.PrinterConvertEncoding)
            {
                pckEncodingTo.IsEnabled = true;
                Encodings.ForEach(x =>
                {
                    if (!pckEncodingTo.Items.Contains(x.EncodingName)) { pckEncodingTo.Items.Add(x.EncodingName); }
                    if (x.EncodingName == App.SFASettings.EncodingTo?.EncodingName) { pckEncodingTo.SelectedIndex = index; }
                    index++;
                });
            }
            else
            {
                pckEncodingTo.IsEnabled = false;
                App.SFASettings.EncodingTo = null;
            }
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private void StoreChanged()
        {
            if (SelectedStoreOid == null || SelectedStoreOid == Guid.Empty)
            {
                pckDocumentType.IsEnabled = false;
                pckDocumentSeries.IsEnabled = false;
                pckLoadingDocumentType.IsEnabled = false;
                pckUnLoadingDocumentType.IsEnabled = false;
                pckUnLoadingDocumentSeries.IsEnabled = false;
            }
            else
            {
                pckDocumentType.IsEnabled = true;
                pckDocumentSeries.IsEnabled = true;
                pckLoadingDocumentType.IsEnabled = true;
                pckUnLoadingDocumentType.IsEnabled = true;
                pckUnLoadingDocumentSeries.IsEnabled = true;
                LoadSfaDocumentTypes();
            }
        }

        private void LoadSfaDocumentTypes()
        {
            StoreDocumentSeriesTypeList = App.DbLayer.GetAllStoreDocumentSeriesTypes(SelectedStoreOid);
            List<Guid> SfaDocumentSeries = DocumentSeries.Where(x => x.eModule == eModule.SFA && x.StoreOid == SelectedStoreOid).Select(x => x.Oid).ToList();
            List<Guid> SfaDocTypeOids = StoreDocumentSeriesTypeList.Where(x => SfaDocumentSeries.Contains(x.DocumentSeriesOid)).Select(x => x.DocumentTypeOid).ToList();
            SfaDocumentTypes = AllDocumentTypes.Where(x => SfaDocTypeOids.Contains(x.Oid)).ToList();
            int index = 0;
            SfaDocumentTypes.ForEach(x =>
            {
                if (!pckDocumentType.Items.Contains(x.Description)) { pckDocumentType.Items.Add(x.Description); }
                if (x.Oid == SelectedDocumentTypeOid) { pckDocumentType.SelectedIndex = index; }
                index++;
            });
        }

        private void LoadDocumentSeries(Guid DocumentTypeOid, Guid serieOid)
        {
            try
            {
                pckDocumentSeries.Items.Clear();
                pckDocumentSeries.IsEnabled = true;
                List<DocumentSeries> AvailiableSeries = new List<DocumentSeries>();
                int indexSeries = 0;
                foreach (StoreDocumentSeriesType storeDocumentSeriesType in StoreDocumentSeriesTypeList.Where(x => x.DocumentTypeOid == DocumentTypeOid))
                {
                    AvailiableSeries = DocumentSeries.Where(x => x.Oid == storeDocumentSeriesType.DocumentSeriesOid).ToList();

                    foreach (DocumentSeries serie in AvailiableSeries.Where(x => x != null).ToList())
                    {
                        if (!pckDocumentSeries.Items.Contains(serie.Description))
                        {
                            pckDocumentSeries.Items.Add(serie.Description);
                        }
                        if (serieOid == null || serieOid == Guid.Empty)
                        {
                            pckDocumentSeries.SelectedIndex = indexSeries;
                        }
                        else if (serieOid == serie.Oid)
                        {
                            pckDocumentSeries.SelectedIndex = indexSeries;
                        }
                        indexSeries++;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private void UpdateUnLoadingDocumentSeries(Guid DocumentTypeOid)
        {
            try
            {
                pckUnLoadingDocumentSeries.Items.Clear();
                pckUnLoadingDocumentSeries.IsEnabled = true;
                List<DocumentSeries> AvailiableSeries = new List<DocumentSeries>();
                foreach (StoreDocumentSeriesType storeDocumentSeriesType in StoreDocumentSeriesTypeList.Where(x => x.DocumentTypeOid == DocumentTypeOid))
                {
                    AvailiableSeries = DocumentSeries.Where(x => x.Oid == storeDocumentSeriesType.DocumentSeriesOid).ToList();
                    int indexSeries = 0;
                    foreach (DocumentSeries serie in AvailiableSeries.Where(x => x != null).ToList())
                    {
                        if (!pckUnLoadingDocumentSeries.Items.Contains(serie.Description))
                        {
                            pckUnLoadingDocumentSeries.Items.Add(serie.Description);
                        }
                        pckUnLoadingDocumentSeries.SelectedIndex = indexSeries;
                        indexSeries++;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        public async void OnSaveSettings(object sender, EventArgs eventArguments)
        {
            string errorMessage = string.Empty;
            if (!ValidateForm(out errorMessage))
            {
                await DisplayAlert(ResourcesRest.Warning, ResourcesRest.PleaseFillInAllRequiredFields + " " + errorMessage, ResourcesRest.MsgBtnOk);
                return;
            }
            else
            {
                if (App.SFASettings == null)
                {
                    App.SFASettings = new SFASettings();
                }
                int timeout;
                int.TryParse(entryTimeout.Text, out timeout);
                App.SFASettings.AuthenticationURL = entryAuthenticationURL.Text;
                App.SFASettings.ServerURL = entryServerURL.Text;
                App.SFASettings.ApiTimeout = timeout;
                App.SFASettings.DatabaseDownloadURL = entryDownloadURL.Text;
                App.SFASettings.CategoryNode = SelectedCategoryOid;
                App.SFASettings.Language = String.IsNullOrEmpty(SelectedLanguage) ? SFAConstants.DEFAULT_LANGUAGE : SelectedLanguage;
                App.SFASettings.DefaultDocumentStatusOid = SelectedDocumentStatusOid;
                App.SFASettings.DocumentStatusToSendOid = SelectedDocumentStatusToSendOid;
                App.SFASettings.DefaultDocumentTypeOid = SelectedDocumentTypeOid;
                App.SFASettings.DocumentSeries = SelectedSeriesOid;
                App.SFASettings.DefaultStore = SelectedStoreOid;
                App.SFASettings.BlueToothScanner = SelectedBlueToothScanner;
                App.SFASettings.BlueToothPrinter = SelectedBlueToothPrinter;
                App.SFASettings.SfaId = SelectedTabletId;
                App.SFASettings.LoadingDocumentTypeOid = SelectedLoadingDocumentTypeOid;
                App.SFASettings.UnLoadingDocumentTypeOid = SelectedUnLoadingDocumentTypeOid;
                App.SFASettings.UnLoadingSeriesOid = SelectedUnLoadingSeriesOid;
                App.SFASettings.Zpl = ZPLSwitch.Checked;
                App.SFASettings.VehicleNumber = entryVehicleNumber.Text;
                int linechars = 80;
                if (!string.IsNullOrWhiteSpace(entryPrinterLineChars.Text))
                {
                    if (int.TryParse(entryPrinterLineChars.Text, out linechars))
                    {
                        App.SFASettings.PrinterLineChars = linechars;
                    }
                }
            }
            try
            {
                string error = string.Empty;
                if (!DependencyService.Get<ICrossPlatformMethods>().ValidateSettings(App.SFASettings, out error))
                {
                    await DisplayAlert(ResourcesRest.Warning, error, ResourcesRest.MsgBtnOk);
                    return;
                }
                else
                {
                    WriteSettingsToXML(App.SFASettings);
                }
                if (DependencyService.Get<ICrossPlatformMethods>().SettingsFileExists())
                {
                    if (App.Login == true)
                    {
                        Application.Current.MainPage = new NavigationPage(new MainPage());
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new LoginPage());
                    }
                }
                else
                {
                    await DisplayAlert(ResourcesRest.Warning, "Fail To Save Settings", ResourcesRest.MsgBtnOk);
                    return;
                }
                App.SFASettings = DependencyService.Get<ICrossPlatformMethods>().ReadSettingsFromXML();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private bool ValidateForm(out string error)
        {
            error = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(entryServerURL.Text))
            {
                sb.AppendLine("Server Url Required");
            }
            if (string.IsNullOrEmpty(entryAuthenticationURL.Text))
            {
                sb.AppendLine("Authentication Url Required");
            }
            if (string.IsNullOrEmpty(entryDownloadURL.Text))
            {
                sb.AppendLine("DatabaseDown Url Required");
            }
            if (SelectedDocumentStatusOid == null || SelectedDocumentStatusOid == Guid.Empty)
            {
                sb.AppendLine("Default Document Status Required");
            }
            if (SelectedDocumentStatusToSendOid == null || SelectedDocumentStatusToSendOid == Guid.Empty)
            {
                sb.AppendLine("Send Document Status Required");
            }
            if (SelectedDocumentTypeOid == null || SelectedDocumentTypeOid == Guid.Empty)
            {
                sb.AppendLine("Default DocumentType Required");
            }
            if (SelectedSeriesOid == null || SelectedSeriesOid == Guid.Empty || SelectedSeriesOid == null)
            {
                sb.AppendLine("DocumentSeries For DocumentType Required");
            }
            if (SelectedStoreOid == null || SelectedStoreOid == Guid.Empty)
            {
                sb.AppendLine("Store  Required");
            }
            if (SelectedTabletId < 0)
            {
                sb.AppendLine("Device Code  Required");
            }
            error = sb.ToString();
            return error == string.Empty ? true : false;
        }

        async void OnCancel(object sender, EventArgs eventArguments)
        {
            try
            {
                if (_PageComeFrom == "Login")
                {
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                }
                else if (_PageComeFrom == "MainPage")
                {
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        protected void WriteSettingsToXML(SFASettings settings)
        {
            DependencyService.Get<ICrossPlatformMethods>().WriteSettingsToXML(settings);
        }

        private async void LoadLanguages()
        {
            try
            {
                if (App.SFASettings != null && !string.IsNullOrEmpty(App.SFASettings.Language))
                {
                    SelectedLanguage = App.SFASettings.Language;
                }
                else
                {
                    SelectedLanguage = "en-GB";
                }
                int index = 0;
                foreach (Language item in ListLanguages)
                {
                    if (!pckLanguageList.Items.Contains(item.Description))
                    {
                        pckLanguageList.Items.Add(item.Description);
                    }
                    if (item.Nationality.Equals(SelectedLanguage))
                    {
                        pckLanguageList.SelectedIndex = index;
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblSettings.Text = ResourcesRest.SettingsLblSettings;
            lblVehicleNumber.Text = ResourcesRest.VehicleNumber;
            lblAuthenticationURL.Text = ResourcesRest.SettingsLblAuthenticationURL;
            lblServerURL.Text = ResourcesRest.SettingsLblServerURL;
            lblDownloadURL.Text = ResourcesRest.DatabaseURLDownload;
            lblSelectedLanguage.Text = ResourcesRest.DefaultLanguage;
            lblSelectedCategoryNode.Text = ResourcesRest.DefaultCategoryNode;
            lblDevices.Text = ResourcesRest.SfaId;
            pckDevices.Title = ResourcesRest.SelectDevice;
            pckLanguageList.Title = ResourcesRest.SettingsPckDefaultLanguage;
            pckSelectedStore.Title = ResourcesRest.SettingPckSelectedStore;
            pckCategoryNodeList.Title = ResourcesRest.SettingsPckCategoryNodeList;
            pckDocumentType.Title = ResourcesRest.DocumentType;
            pckLoadingDocumentType.Title = ResourcesRest.LoadingDocumentType;
            pckUnLoadingDocumentType.Title = ResourcesRest.UnLoadingDocumentType;
            pckDocumentSeries.Title = ResourcesRest.DocumentSeries;
            pckUnLoadingDocumentSeries.Title = ResourcesRest.UnLoadingDocumentSeries;
            lblPairScannerDevices.Text = ResourcesRest.BlueToothScanner;
            lblPairPrinterDevices.Text = ResourcesRest.BlueToothPrinter;
            btnSave.Text = ResourcesRest.Save;
            btnCancel.Text = ResourcesRest.SettingsBtnCancel;
            strOk = ResourcesRest.MsgBtnOk;
            MsgSaveSettingsToContinue = ResourcesRest.MsgSaveSettingsToContinue;
            lblSelectedDocumentSeries.Text = ResourcesRest.DefaultDocumentSeries;
            lblSelectedDocumentType.Text = ResourcesRest.DefaultDocumentType;
            lblSelectedStore.Text = ResourcesRest.DefaultStrore;
            lblSelectedDocumentStatus.Text = ResourcesRest.DefaultDocumentStatus;
            pckDocumentStatus.Title = ResourcesRest.pckLblSelectDefaultDocumentStatus;
            lblSelectedDocumentStatusToSend.Text = ResourcesRest.DefaultDocumentStatusToSend;
            pckDocumentStatusToSend.Title = ResourcesRest.pckLblSelectDocumentSatusToSend;
            lblPrinterConvertEncoding.Text = "Enable Printer Encoding";
            pckEncodingFrom.Title = "Encoding From";
            pckEncodingTo.Title = "Encoding To";
            lblEncodingFrom.Text = "Encoding From";
            lblEncodingTo.Text = "Encoding To";
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
            btnExportDb.Text = ResourcesRest.ExportDatabase;
            btnImportDb.Text = ResourcesRest.ImportDatabase;
            btnDownloadDb.Text = ResourcesRest.DownloadDatabase;
            btnTestScannerDevice.Text = "Connect To Scanner";
            btnTestPrinterDevice.Text = "Connect To Printer";
            lblLoadingDocumentType.Text = ResourcesRest.LoadingDocument;
            lblUnLoadingDocumentType.Text = ResourcesRest.UnLoadingDocument;
            lblUnLoadingDocumentSeries.Text = ResourcesRest.UnLoadingDocumentSeries;
            lblTimeout.Text = "Web Api Timeout (seconds)";
            lblPrinterLineChars.Text = ResourcesRest.PrinterLineChars;
        }

        protected override void OnDisappearing()
        {
            DisConnectBlueToothScanner();
            DisConnectBlueToothPrinter();
            base.OnDisappearing();
        }

        public async void OnTestScannerDevice(object sender, EventArgs args)
        {
            try
            {
                App.SFASettings.BlueToothScanner = SelectedBlueToothScanner;
                if (string.IsNullOrEmpty(App.SFASettings.BlueToothScanner))
                {
                    await DisplayAlert(strMsgTypeAlert, strMsgFailure + "Device Not Found", strMsgOk);
                }
                else
                {
                    _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                    if (_IsConnected)
                    {
                        await DisplayAlert(strMsgTypeAlert, "Device Already Connected, Scan a Barcode", strMsgOk);
                    }
                    else
                    {
                        UserDialogs.Instance.ShowLoading("Connecting..", MaskType.Black);
                        await Task.Run(() =>
                        {
                            DisConnectBlueToothScanner();
                            ConnectBlueToothScanner();
                            _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                            int seconds = 20;
                            do
                            {
                                Thread.Sleep(1000);
                                seconds--;
                                _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                            } while (_IsConnected == false && seconds > 0);
                        });
                        UserDialogs.Instance.HideLoading();
                        _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                        if (_IsConnected)
                        {
                            await DisplayAlert(strMsgTypeAlert, "Device Is Connected, Scan a Barcode", strMsgOk);
                            btnTestScannerDevice.Text = "Scan a Barcode";
                        }
                        else
                        {
                            await DisplayAlert(strMsgTypeAlert, "Device Is Not Connected", strMsgOk);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        public void OnPrinterConvertEncodingChanged(object sender, EventArgs args)
        {
            if (chkPrinterConvertEncoding.Checked)
            {
                App.SFASettings.PrinterConvertEncoding = true;
                pckEncodingFrom.IsEnabled = true;
                pckEncodingTo.IsEnabled = true;
                LoadEncodingPickers();
            }
            else
            {
                App.SFASettings.PrinterConvertEncoding = false;
                pckEncodingFrom.IsEnabled = false;
                pckEncodingTo.IsEnabled = false;
                pckEncodingFrom.SelectedIndex = -1;
                pckEncodingTo.SelectedIndex = -1;
                App.SFASettings.EncodingFrom = null;
                App.SFASettings.EncodingTo = null;

            }
        }

        public async void OnTestPrinterDevice(object sender, EventArgs args)
        {
            try
            {
                App.SFASettings.BlueToothPrinter = SelectedBlueToothPrinter;
                if (string.IsNullOrEmpty(App.SFASettings.BlueToothPrinter))
                {
                    await DisplayAlert(strMsgTypeAlert, strMsgFailure + "Device Not Found", strMsgOk);
                }
                else
                {
                    _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                    if (_IsConnected)
                    {
                        btnTestPrinterDevice.Text = "Print Test Page";
                        bool printTestPage = await DisplayAlert(strMsgTypeAlert, "Printer Already Connected. Do You Want To Print Test Page?", strMsgOk, "Cancel");
                        {
                            await DependencyService.Get<IBlueTooth>().PrintTestPage(SelectedBlueToothPrinter);
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.ShowLoading("Connecting..", MaskType.Black);
                        await Task.Run(() =>
                        {
                            DisConnectBlueToothPrinter();
                            ConnectBlueToothPrinter(SelectedBlueToothPrinter);
                            _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                            int seconds = 20;
                            do
                            {
                                Thread.Sleep(1000);
                                seconds--;
                                _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                            } while (_IsConnected == false && seconds > 0);
                        });
                        UserDialogs.Instance.HideLoading();
                        _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                        if (_IsConnected)
                        {
                            btnTestPrinterDevice.Text = "Print Test Page " + Environment.NewLine + "δοκιμαστικη σελιδα";
                            await DisplayAlert(strMsgTypeAlert, "Device Is Connected", strMsgOk);
                        }
                        else
                        {
                            await DisplayAlert(strMsgTypeAlert, "Device Is Not Connected", strMsgOk);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        private void DisConnectBlueToothPrinter()
        {
            DependencyService.Get<IBlueTooth>().Cancel();
            _IsConnected = false;
        }

        private void ConnectBlueToothPrinter(string SelectedBluetooth)
        {
            DependencyService.Get<IBlueTooth>().Start(SelectedBluetooth, eBlueToothDevice.PRINTER);
        }

        private void ConnectBlueToothScanner()
        {
            DependencyService.Get<IBlueTooth>().Start(App.SFASettings.BlueToothScanner, eBlueToothDevice.SCANNER);
            MessagingCenter.Subscribe<App, string>(this, EventNames.SCAN_EVENT, (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, arg, strMsgOk);
                });
            });
        }

        private void DisConnectBlueToothScanner()
        {
            DependencyService.Get<IBlueTooth>().Cancel();
            MessagingCenter.Unsubscribe<App, string>(this, EventNames.SCAN_EVENT);
            _IsConnected = false;
        }

        async void OnExportDB(object sender, EventArgs e)
        {
            try
            {
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.DoYouWantToExportDatabase, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (answer == true)
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.ExportingDatabasePleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        DependencyService.Get<ICrossPlatformMethods>().ExportDB();
                    });
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.ExportDatabaseCompletedSuccessful, strMsgOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                UserDialogs.Instance.HideLoading();
            }
        }

        async void OnImportDB(object sender, EventArgs e)
        {
            try
            {
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.DoYouWantToImportDatabase, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (answer == true)
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.ImportingDatabasePleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        DependencyService.Get<ICrossPlatformMethods>().ImportDB();
                    });
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.ImportDatabaseCompletedSuccessful, strMsgOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                UserDialogs.Instance.HideLoading();
            }
        }

        async void OnDownloadDB(object sender, EventArgs e)
        {
            try
            {
                AndroidMethods androidLayer = new AndroidMethods();
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.AreYouSureToDownloadDatabase, ResourcesRest.MsgYes, ResourcesRest.MsgNo);

                if (answer == true)
                {


                    Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus> permissions = new Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus>();

                    var check = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    var shouldAsk = await Plugin.Permissions.CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                    if (check != Plugin.Permissions.Abstractions.PermissionStatus.Granted || shouldAsk == true)
                    {
                        permissions = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    }
                    else
                    {
                        permissions.Add(Plugin.Permissions.Abstractions.Permission.Storage, Plugin.Permissions.Abstractions.PermissionStatus.Granted);
                    }

                    bool value = permissions.TryGetValue(Plugin.Permissions.Abstractions.Permission.Storage, out check);

                    if (value == false || check != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            UserDialogs.Instance.HideLoading();
                            DisplayAlert(strMsgTypeAlert, strMsgFailure + "Application Require Storage Permissions", strMsgOk);
                            return;
                        });
                        return;
                    }

                    int step = 1;
                    int steps = 4;
                    UserDialogs.Instance.ShowLoading(ResourcesRest.DownloadingDatabasePleaseWait + " " + ResourcesRest.Step + ": " +
                                                                        step + " " + ResourcesRest.of + " " + steps, MaskType.Black);

                    var finalPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                    await Task.Run(() =>
                    {
                        try
                        {
                            DependencyService.Get<ICrossPlatformMethods>().DownloadDB(entryDownloadURL.Text);
                        }
                        catch (Exception ex)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                UserDialogs.Instance.HideLoading();
                                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                            });
                            return;
                        }
                    });

                    step++;
                    UserDialogs.Instance.ShowLoading(ResourcesRest.UnzipFile + " " + ResourcesRest.Step + ": " + step + " " + ResourcesRest.of + " " + steps, MaskType.Black);
                    await Task.Run(() =>
                    {
                        DependencyService.Get<ICrossPlatformMethods>().UnzipFile();
                    });
                    UserDialogs.Instance.HideLoading();
                    step++;
                    UserDialogs.Instance.ShowLoading(ResourcesRest.ImportingDatabasePleaseWait + " " + ResourcesRest.Step + ": " + step + " " + ResourcesRest.of + " " + steps, MaskType.Black);
                    await Task.Run(() =>
                    {
                        DependencyService.Get<ICrossPlatformMethods>().ImportDB();
                    });
                    UserDialogs.Instance.HideLoading();
                    step++;
                    UserDialogs.Instance.ShowLoading(ResourcesRest.MigrationDatabase + " " + ResourcesRest.Step + ": " + step + " " + ResourcesRest.of + " " + steps, MaskType.Black);
                    await Task.Run(() =>
                    {
                        DependencyService.Get<ICrossPlatformMethods>().GenerateDatabase();
                    });
                    InitialiseOptions(true);
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.ImportDatabaseCompletedSuccessful, strMsgOk);
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                });
            }
        }

        public void OnServerUrlFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnServerUrlUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnAuthUrlFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnAuthUrlUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnDBUrlFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnDBUrlUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnTimeoutFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnTimeoutUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnVehicleNumberFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnVehicleNumberUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnPrinterLineCharsFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnPrinterLineCharsUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void CheckBoxChecked(object sender, EventArgs args)
        {
            if (ZPLSwitch.Checked)
            {
                App.SFASettings.Zpl = true;
            }
            else
            {
                App.SFASettings.Zpl = false;
            }
        }
    }
}
