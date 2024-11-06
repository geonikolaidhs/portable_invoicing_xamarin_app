using Acr.UserDialogs;
using Plugin.Permissions.Abstractions;
using ITS.WRM.SFA.Droid.Classes;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model;
using System.Text;
using ITS.WRM.SFA.Model.NonPersistant;

namespace ITS.WRM.SFA.Droid.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private List<ItemCategory> ItemCategories = new List<ItemCategory>();
        private List<Language> ListLanguages = new List<Language>() { new Language() { Nationality = "en-GB", Description = "English" }, new Language() { Nationality = "el-GR", Description = "Ελληνικά" } };
        private List<DocumentStatus> ListDocumentStatus = new List<DocumentStatus>();
        private List<DocumentType> DocumentTypes = new List<DocumentType>();
        private List<Store> Stores = new List<Store>();
        private List<DocumentSeries> DocumentSeries = new List<DocumentSeries>();

        private static string OnFocusColor { get; set; } = "#a3d8e8";
        private static string OnUnFocusColor { get; set; } = "#f5f5f5";
        public List<OrderHeader> DocHeader { get; private set; }

        private Guid SelectedCategoryOid;
        private Guid SelectedStoreOid;
        private Guid SelectedSeriesOid;
        private Guid SelectedDocumentTypeOid;
        private Guid SelectedDocumentStatusOid;
        private Guid SelectedDocumentStatusToSendOid;
        private string SelectedLanguage;



        private string strOk;
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        private string _SelectedLanguage;

        public string MsgSaveSettingsToContinue { get; private set; }
        public string _PageComeFrom;

        public SettingsPage()
        {
            InitializeComponent();
        }
        public SettingsPage(string PageComeFrom)
        {
            try
            {
                _PageComeFrom = PageComeFrom;
                DependencyService.Get<ICrossPlatformMethods>().LogInfo("Initaliasing Login Page");
                InitializeComponent();
                DependencyService.Get<ICrossPlatformMethods>().LogInfo("Initiallising Controllers");
                InitiallizeControllers();
                DependencyService.Get<ICrossPlatformMethods>().LogInfo("Initiallising Options");
                InitialiseOptions();

                pckDocumentType.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckDocumentType.SelectedIndex == -1)
                    {
                        SelectedDocumentTypeOid = Guid.Empty;
                    }
                    else
                    {
                        string DocumentTypeDescription = pckDocumentType.Items[pckDocumentType.SelectedIndex];
                        SelectedDocumentTypeOid = DocumentTypes.Where(x => x.Description == DocumentTypeDescription).FirstOrDefault()?.Oid ?? Guid.Empty;
                        SelectedSeriesOid = Guid.Empty;
                        pckDocumentSeries.SelectedIndex = -1;
                    }
                    LoadDocumentSeries(SelectedDocumentTypeOid);
                };

                pckDocumentSeries.SelectedIndexChanged += (sender, args) =>
                {
                    if (pckDocumentSeries.SelectedIndex == -1)
                    {
                        SelectedSeriesOid = Guid.Empty;
                    }
                    else
                    {
                        string SeriesName = pckDocumentSeries.Items[pckDocumentSeries.SelectedIndex];
                        SelectedSeriesOid = DocumentSeries.Where(xx => xx != null).ToList()?.Where(x => x.Code == SeriesName).FirstOrDefault()?.Oid ?? Guid.Empty;
                    }
                };



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
                        _SelectedLanguage = App.SFASettings.Language;
                    }
                };
            }
            catch (Exception ex)
            {
                DependencyService.Get<ICrossPlatformMethods>().LogError(ex);
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
            if (string.IsNullOrEmpty(App.SFASettings.AuthenticationURL))
            {
                entryAuthenticationURL.Text = @"http://10.3.1.154/apisfa/Token";
            }

            if (string.IsNullOrEmpty(App.SFASettings.ServerURL))
            {
                entryServerURL.Text = @"http://10.3.1.154/apisfa/api";
            }

            if (string.IsNullOrEmpty(App.SFASettings.DatabaseDownloadURL))
            {
                entryDownloadURL.Text = @"http://10.3.1.154/sfa2";
            }
        }

        private void InitialiseOptions()
        {
            try
            {
                swiScan.IsEnabled = true;
                entryAuthenticationURL.Text = App.SFASettings.AuthenticationURL;
                entryServerURL.Text = App.SFASettings.ServerURL;
                entryDownloadURL.Text = App.SFASettings.DatabaseDownloadURL;
                SelectedCategoryOid = App.SFASettings.CategoryNode;
                SelectedStoreOid = App.SFASettings.DefaultStore;
                SelectedSeriesOid = App.SFASettings.DocumentSeries;
                SelectedDocumentTypeOid = App.SFASettings.DefaultDocumentTypeOid;
                SelectedDocumentStatusOid = App.SFASettings.DefaultDocumentStatusOid;
                SelectedDocumentStatusToSendOid = App.SFASettings.DocumentStatusToSendOid;
                SelectedSeriesOid = App.SFASettings.DocumentSeries;
                SelectedLanguage = App.SFASettings.Language;
                swiScan.IsToggled = App.SFASettings.Scan;
                PrintService.Text = App.SFASettings.PrintService;
                DsignService.Text = App.SFASettings.DsignService;
                DependencyService.Get<ICrossPlatformMethods>().LogInfo("Loading Languages");
                LoadLanguages();

                SetSomeValues();
                ///LOAD & SET SELECTED ITEMCATEGORY///
                int index = 0;
                ItemCategories = App.DbLayer.GetAllRootCategoryNodes();
                ItemCategories.ForEach(x =>
                {
                    if (!pckCategoryNodeList.Items.Contains(x.Description)) { pckCategoryNodeList.Items.Add(x.Description); }
                    if (x.Oid == SelectedCategoryOid) { pckCategoryNodeList.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET SELECTED DEFAULT DOCUMENTSTATUS & DOCUMENTSTATUS TO SEND///
                index = 0;
                ListDocumentStatus = App.DbLayer.GetAllDocumentStatus();
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
                Stores = App.DbLayer.GetAllStores();
                Stores.ForEach(x =>
                {
                    if (!pckSelectedStore.Items.Contains(x.Name)) { pckSelectedStore.Items.Add(x.Name); }
                    if (x.Oid == SelectedStoreOid) { pckSelectedStore.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET  DOCUMENT TYPE///
                index = 0;
                DocumentTypes = App.DbLayer.GetAllValidDocumentTypes();
                DocumentTypes.ForEach(x =>
                {
                    if (!pckDocumentType.Items.Contains(x.Description)) { pckDocumentType.Items.Add(x.Description); }
                    if (x.Oid == SelectedDocumentTypeOid) { pckDocumentType.SelectedIndex = index; }
                    index++;
                });

                ///LOAD & SET  DOCUMENT SERIES TYPE///
                index = 0;
                DocumentSeries = App.DbLayer.GetAllDocumentSeries();
                if (SelectedDocumentTypeOid != null && SelectedDocumentTypeOid != Guid.Empty)
                {
                    pckDocumentSeries.IsEnabled = true;
                    DocumentSeries.ForEach(x =>
                    {
                        if (!pckDocumentSeries.Items.Contains(x.Description)) { pckDocumentSeries.Items.Add(x.Description); }
                        if (x.Oid == SelectedSeriesOid) { pckDocumentSeries.SelectedIndex = index; }
                        index++;
                    });
                    LoadDocumentSeries(SelectedDocumentTypeOid);
                }
                else
                {
                    pckDocumentSeries.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private void LoadDocumentSeries(Guid DocumentTypeOid)
        {
            try
            {
                pckDocumentSeries.Items.Clear();
                pckDocumentSeries.IsEnabled = true;
                List<StoreDocumentSeriesType> StoreDocumentSeriesTypeList = App.DbLayer.GetAllStoreDocumentSeriesTypes();
                List<DocumentSeries> AvailiableSeries = new List<DocumentSeries>();
                foreach (StoreDocumentSeriesType storeDocumentSeriesType in StoreDocumentSeriesTypeList.Where(x => x.DocumentTypeOid == DocumentTypeOid))
                {
                    AvailiableSeries = DocumentSeries.Where(x => x.Oid == storeDocumentSeriesType.DocumentSeriesOid).ToList();

                    int indexSeries = 0;
                    foreach (DocumentSeries serie in AvailiableSeries.Where(x => x != null).ToList())
                    {
                        if (!pckDocumentSeries.Items.Contains(serie.Code))
                        {
                            pckDocumentSeries.Items.Add(serie.Code);
                        }
                        pckDocumentSeries.SelectedIndex = indexSeries;
                        indexSeries++;
                    }
                }
            }
            catch (Exception ex)
            {
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

                App.SFASettings.AuthenticationURL = entryAuthenticationURL.Text;
                App.SFASettings.ServerURL = entryServerURL.Text;
                App.SFASettings.PrintService = PrintService.Text;
                App.SFASettings.DsignService = DsignService.Text;
                App.SFASettings.DatabaseDownloadURL = entryDownloadURL.Text;
                App.SFASettings.Scan = swiScan.IsEnabled;
                App.SFASettings.CategoryNode = SelectedCategoryOid;
                App.SFASettings.Language = String.IsNullOrEmpty(_SelectedLanguage) ? SFAConstants.DEFAULT_LANGUAGE : _SelectedLanguage;
                App.SFASettings.DefaultDocumentStatusOid = SelectedDocumentStatusOid;
                App.SFASettings.DocumentStatusToSendOid = SelectedDocumentStatusToSendOid;
                App.SFASettings.DefaultDocumentTypeOid = SelectedDocumentTypeOid;
                App.SFASettings.DocumentSeries = SelectedSeriesOid;
                App.SFASettings.DefaultStore = SelectedStoreOid;
                App.SFASettings.Scan = swiScan.IsToggled ? true : false;

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
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }

        }

        protected void WriteSettingsToXML(SFASettings sfaCommonConstants)
        {
            DependencyService.Get<ICrossPlatformMethods>().WriteSettingsToXML(sfaCommonConstants);
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
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        private void InitiallizeControllers()
        {
            string LanguageId = App.SFASettings.Language;
            if (LanguageId == null)
            {
                LanguageId = "en-GB";
            }
            CultureInfo ci = null;
            ci = DependencyService.Get<ICrossPlatformMethods>().Languageinfo(LanguageId);
            ResourcesRest.Culture = ci;

            lblSettings.Text = ResourcesRest.SettingsLblSettings;
            lblAuthenticationURL.Text = ResourcesRest.SettingsLblAuthenticationURL;
            lblServerURL.Text = ResourcesRest.SettingsLblServerURL;
            lblDownloadURL.Text = ResourcesRest.DatabaseURLDownload;
            lblSelectedLanguage.Text = ResourcesRest.DefaultLanguage;
            lblSelectedCategoryNode.Text = ResourcesRest.DefaultCategoryNode;

            pckLanguageList.Title = ResourcesRest.SettingsPckDefaultLanguage;
            pckSelectedStore.Title = ResourcesRest.SettingPckSelectedStore;
            pckCategoryNodeList.Title = ResourcesRest.SettingsPckCategoryNodeList;
            pckDocumentType.Title = ResourcesRest.DocumentType;
            pckDocumentSeries.Title = ResourcesRest.DocumentSeries;
            lblSwitch.Text = ResourcesRest.SettingsSwiScan;
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

            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
            btnExportDb.Text = ResourcesRest.ExportDatabase;
            btnImportDb.Text = ResourcesRest.ImportDatabase;
            btnDownloadDb.Text = ResourcesRest.DownloadDatabase;
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
                UserDialogs.Instance.HideLoading();
                if (answer == true)
                {
                    int step = 1;
                    int steps = 4;
                    var permisssionStatus = await androidLayer.HasPermission(Plugin.Permissions.Abstractions.Permission.Storage);
                    if (permisssionStatus != PermissionStatus.Granted)
                    {
                        Dictionary<Permission, PermissionStatus> results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                        if (results.ContainsKey(Permission.Storage))
                        {
                            permisssionStatus = results[Permission.Storage];
                            if (permisssionStatus != PermissionStatus.Granted)
                            {
                                await DisplayAlert(ResourcesRest.MsgTypeAlert, "Provide Storage Permission To Download Database", ResourcesRest.MsgBtnOk);
                            }
                        }
                    }

                    UserDialogs.Instance.ShowLoading(ResourcesRest.DownloadingDatabasePleaseWait + " " + ResourcesRest.Step + ": " + step + " " + ResourcesRest.of + " " + steps, MaskType.Black);

                    var finalPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                    await Task.Run(() =>
                    {
                        DependencyService.Get<ICrossPlatformMethods>().DownloadDB(entryDownloadURL.Text);
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
                    InitialiseOptions();
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.ImportDatabaseCompletedSuccessful, strMsgOk);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                UserDialogs.Instance.HideLoading();
            }

        }

        public void ScanToogled(object sender, ToggledEventArgs args)
        {
            //Switch item = (Switch)sender;
            //item.IsToggled = item.IsToggled == true ? false : true;
        }

        public void OnServerUrlFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnFocusColor);
        }

        public void OnServerUrlUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnUnFocusColor);
        }

        public void OnAuthUrlFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnFocusColor);
        }

        public void OnAuthUrlUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnUnFocusColor);
        }

        public void OnDBUrlFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnFocusColor);
        }

        public void OnDBUrlUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnUnFocusColor);
        }

        public void OnPrintServiceFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnFocusColor);
        }

        public void OnPrintServiceUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnUnFocusColor);
        }

        public void OnDsignServiceFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnFocusColor);
        }

        public void DsignServiceUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(OnUnFocusColor);
        }







    }
}