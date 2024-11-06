using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Droid.Pages.Maps;
using ITS.WRM.SFA.Droid.Pages.SettingsChiledViews;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;


namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private string strMsgTypeUpload;
        private string strMsgTypeSync;
        private string strMsgOk;
        private string strMsgTypeAlert;
        private string strMsgUpload;
        private string strMsgNo;
        private string strMsgSingOut;
        private string strMsgYes;
        private string strMsgTabNotYet;
        private string strMsgSync;
        private string strMsgTypeSingOut;
        private string strMsgFailure;
        private string strMsgSuccesfullSend;
        private string strMsgNoDataFound;
        private string strMsgSendError;
        private string strSuccessfulSync;
        private string strMsgException;
        private string strMsgNoDocumentSending;
        private DocumentType DefaultDocumentType;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            try
            {
                DefaultDocumentType = App.DocumentTypes.Where(x => x.Oid == App.SFASettings.DefaultDocumentTypeOid).FirstOrDefault();// App.DbLayer.GetDocumentTypeById(App.SFASettings.DefaultDocumentTypeOid);
                if (DefaultDocumentType == null)
                {
                    DefaultDocumentType = App.DbLayer.GetDocumentTypeById(App.SFASettings.DefaultDocumentTypeOid);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Thread.Sleep(2000);
                try
                {
                    DefaultDocumentType = App.DbLayer.GetDocumentTypeById(App.SFASettings.DefaultDocumentTypeOid);
                }
                catch (Exception ex2)
                {
                    App.LogError(ex2);
                }
            }
            if (DefaultDocumentType == null)
            {
                DisplayAlert(strMsgTypeAlert, ResourcesRest.DefaultDocumentType + ResourcesRest.NoResultsFoundMessage, strMsgOk);
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(SettingsPage))
                {
                    Navigation.PushAsync(new SettingsTabPage("MainPage"));
                }
            }
            try
            {
                Microsoft.AppCenter.AppCenter.SetUserId(App.UserName + " (" + App.UserId.ToString() + ")");
                InitiallizeControllers();
                Title = "user: " + App.UserName;
                NavigationPage.SetHasBackButton(this, false);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Navigation.PushAsync(new SettingsTabPage("MainPage"));
            }
        }

        async void OnNewDefaultDocumentClick(object sender, EventArgs e)
        {

            btnNewDefaultDocument.IsEnabled = true;
            btnDocuments.IsEnabled = false;
            btnCustomer.IsEnabled = false;
            btnProducts.IsEnabled = false;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnSettings.IsEnabled = false;
            btnLogout.IsEnabled = false;
            btnPortable.IsEnabled = false;
            btnMaps.IsEnabled = false;
            try
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(DocumentCustomerPage))
                {
                    await Navigation.PushAsync(new DocumentCustomerPage(DefaultDocumentType), true);
                }
                else
                {
                    if (stack[stack.Count - 1].GetType() != typeof(SettingsTabPage))
                    {
                        await Navigation.PushAsync(new SettingsTabPage("MainPage"));
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }

        async void OnDocumentsClick(object sender, EventArgs e)
        {
            btnNewDefaultDocument.IsEnabled = false;
            btnDocuments.IsEnabled = true;
            btnCustomer.IsEnabled = false;
            btnProducts.IsEnabled = false;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnSettings.IsEnabled = false;
            btnLogout.IsEnabled = false;
            btnPortable.IsEnabled = false;
            btnMaps.IsEnabled = false;
            try
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(DocumentListPage))
                {
                    await Navigation.PushAsync(new DocumentListPage(), true);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }

        public async void OnProductsClick(object sender, EventArgs e)
        {
            btnNewDefaultDocument.IsEnabled = false;
            btnDocuments.IsEnabled = false;
            btnCustomer.IsEnabled = false;
            btnProducts.IsEnabled = true;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnSettings.IsEnabled = false;
            btnLogout.IsEnabled = false;
            btnPortable.IsEnabled = false;
            btnMaps.IsEnabled = false;
            try
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(ProductCategoriesPage))
                {
                    await Navigation.PushAsync(new ProductCategoriesPage(), true);

                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }


        async void OnLogOutClick(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(strMsgTypeSingOut, strMsgSingOut, strMsgYes, strMsgNo);
            if (answer == true)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    //LoginPage.DeleteCredentials();
                    App.Login = false;
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                });
            }
        }
        async void OnCustomerClick(object sender, EventArgs e)
        {
            btnNewDefaultDocument.IsEnabled = false;
            btnDocuments.IsEnabled = false;
            btnCustomer.IsEnabled = true;
            btnProducts.IsEnabled = false;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnSettings.IsEnabled = false;
            btnLogout.IsEnabled = false;
            btnMaps.IsEnabled = false;
            btnPortable.IsEnabled = false;
            try
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(CustomerListPage))
                {
                    await Navigation.PushAsync(new CustomerListPage(), true);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }
        async void OnSettingsClick(object sender, EventArgs e)
        {
            btnNewDefaultDocument.IsEnabled = false;
            btnDocuments.IsEnabled = false;
            btnCustomer.IsEnabled = false;
            btnProducts.IsEnabled = false;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnSettings.IsEnabled = true;
            btnLogout.IsEnabled = false;
            btnPortable.IsEnabled = false;
            btnMaps.IsEnabled = false;
            try
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(SettingsPage))
                {
                    await Navigation.PushAsync(new SettingsTabPage("MainPage"), true);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }

        async void OnPortableClick(object sender, EventArgs e)
        {
            btnNewDefaultDocument.IsEnabled = false;
            btnDocuments.IsEnabled = false;
            btnCustomer.IsEnabled = false;
            btnProducts.IsEnabled = false;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnPortable.IsEnabled = true;
            btnLogout.IsEnabled = false;
            btnMaps.IsEnabled = false;
            try
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(PortableInvoicingTabPage))
                {
                    await Navigation.PushAsync(new PortableInvoicingTabPage(), true);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            btnPortable.Text = ResourcesRest.PortableInvoicing;
            btnCustomer.Text = ResourcesRest.MenuCustomer;
            btnLogout.Text = ResourcesRest.MenuLogOut;
            btnNewDefaultDocument.Text = DefaultDocumentType.Description;
            btnDocuments.Text = ResourcesRest.Documents;
            btnProducts.Text = ResourcesRest.MenuProducts;
            btnSend.Text = ResourcesRest.MenuSend;
            btnSettings.Text = ResourcesRest.SettingsLblSettings;
            btnMaps.Text = ResourcesRest.Routes;
            btnSync.Text = ResourcesRest.MenuSynchronization;
            strMsgTypeUpload = ResourcesRest.MsgTypeUpload;
            strMsgTypeSync = ResourcesRest.MsgTypeSync;
            strMsgOk = ResourcesRest.MsgBtnOk;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgUpload = ResourcesRest.MainPageMsgUpload;
            strMsgSingOut = ResourcesRest.MainPageMsgSingOut;
            strMsgNo = ResourcesRest.MsgNo;
            strMsgYes = ResourcesRest.MsgYes;
            strMsgTabNotYet = ResourcesRest.MainPageMsgTab;
            strMsgSync = ResourcesRest.MainPageMsgSync;
            strMsgTypeSingOut = ResourcesRest.MsgTypeSingOut;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgSuccesfullSend = ResourcesRest.sendMsgSuccessSend;
            strMsgNoDataFound = ResourcesRest.sendMsgNoDataFound;
            strMsgSendError = ResourcesRest.SendError;
            strSuccessfulSync = ResourcesRest.msgSuccessSynchronize;
            strMsgException = ResourcesRest.sendExceptionMsg;
            strMsgNoDocumentSending = ResourcesRest.strMsgNoDocumentSending;
        }

        async void OnSyncClick(object sender, EventArgs e)
        {
            List<DocumentHeader> documentHeaderList = App.DbLayer.GetUnSyncedDocumentHeaders(App.SFASettings.DocumentStatusToSendOid, App.UserId);
            if (documentHeaderList.Count > 0)
            {
                await DisplayAlert(strMsgTypeAlert, ResourcesRest.AllOpenDocumentsToSync, strMsgOk);
                return;
            }
            var answer = await DisplayAlert(strMsgTypeSync, strMsgSync, strMsgYes, strMsgNo);
            if (answer == true)
            {
                await SyncDatabase();
                UserDialogs.Instance.Progress().Hide();
                await DisplayAlert(strMsgTypeAlert, strSuccessfulSync, strMsgOk);
            }
        }

        async void OnMapsClicked(object sender, EventArgs e)
        {
            btnNewDefaultDocument.IsEnabled = false;
            btnDocuments.IsEnabled = false;
            btnCustomer.IsEnabled = false;
            btnProducts.IsEnabled = false;
            btnSync.IsEnabled = false;
            btnSend.IsEnabled = false;
            btnSettings.IsEnabled = false;
            btnLogout.IsEnabled = false;
            btnMaps.IsEnabled = true;
            btnPortable.IsEnabled = false;

            try
            {
                if (await HasLocationPermissions())
                {
                    string apikey = App.DbLayer.GetAllSfaDevices().Where(x => x.ID == App.SFASettings.SfaId).FirstOrDefault()?.GoogleApiKey;
                    if (string.IsNullOrEmpty(apikey))
                    {
                        throw new Exception("Google Api Key Not Found, Please Register the key in WRM-SFA Settings");
                    }
                    UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                    AddressLocation location = null;
                    location = await LocationManager.GetCurrentlocation();
                    if (location == null)
                    {
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + "This Application Feature Require Location, " +
                                                                                                " Please enable GPS and retry", ResourcesRest.MsgBtnOk);
                        return;
                    }
                    Position currentPos = new Position(location.Latitude, location.Longitude);
                    List<CustomPin> pins = App.DbLayer.GetAllAddressWithLocation();
                    IReadOnlyList<Page> stack = Navigation.NavigationStack;
                    if (stack[stack.Count - 1].GetType() != typeof(MapMaster))
                    {
                        await Navigation.PushAsync(new MapMaster(pins, currentPos, apikey), true);
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + "Application Require Location Permissions", ResourcesRest.MsgBtnOk);
                        return;
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
                EnabledButtons();
                return;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task<bool> SyncDatabase()
        {
            UserDialogs.Instance.Progress("Contacting Server..");
            await ApiHelper.Authenticate(App.UserName, App.UserPassword);

            bool completed = false;
            int countSyncTable = 0;
            List<Type> DownloadTypes = DependencyService.Get<ICrossPlatformMethods>().GetTypesToUpdate() ?? new List<Type>();
            int tblCount = DownloadTypes.Count();

            foreach (Type type in DownloadTypes)
            {
                try
                {
                    countSyncTable++;
                    string msg = ResourcesRest.MainPageMsgDownloadData + " " + countSyncTable + " " + ResourcesRest.From + " " + tblCount.ToString() + " Table(" + type.Name + ")";
                    msg = msg.PadRight(100);
                    double percent = ((double)countSyncTable / (double)tblCount) * 100;
                    UserDialogs.Instance.Progress(msg).PercentComplete = (int)percent;
                    await Task.Delay(150);
                    await DependencyService.Get<ICrossPlatformMethods>().SyncTableAsync(type);

                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    await App.Refresh();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(strMsgTypeAlert, strMsgException + type.Name + "  " + ex.Message, strMsgOk);
                        UserDialogs.Instance.Progress().Hide();
                    });
                    break;
                }
            }
            await App.Refresh();
            completed = true;
            return completed;

        }

        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");

        async Task SuccessAuthedicate()
        {
            HttpRequests httpRequest = new HttpRequests();
            string content = await httpRequest.Get("/User?$filter=UserName eq '" + App.UserName + "'");
            List<User> CurrentUser = JsonConvert.DeserializeObject<List<User>>(JObject.Parse(content)["value"].ToString());
        }

        public async void OnUploadClick(object sender, EventArgs e)
        {
            try
            {
                bool IsWiFiOpen = DependencyService.Get<ICrossPlatformMethods>().IsConnected();
                if (IsWiFiOpen)
                {
                    if (App.token == null)
                    {
                        HttpResponseMessage response = await ApiHelper.Authenticate(App.UserName, App.UserPassword);
                        if (response == null || response.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                        {
                            await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.MsgThereWasAProblemConnecting, ResourcesRest.MsgBtnOk);
                            return;
                        }
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.BadRequest:
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(ResourcesRest.MsgTypeLoginFailed, ResourcesRest.LoginMsgFailureLoginBadRequest, ResourcesRest.MsgBtnOk);
                                });
                                break;
                            case System.Net.HttpStatusCode.ServiceUnavailable:
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(ResourcesRest.MsgTypeLoginFailed, ResourcesRest.strMsgServiceUnavailable, ResourcesRest.MsgBtnOk);
                                });
                                break;
                            case System.Net.HttpStatusCode.OK:
                                sendData();
                                break;
                            default:
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(ResourcesRest.MsgTypeLoginFailed, ResourcesRest.strMsgServiceUnavailable, ResourcesRest.MsgBtnOk);
                                });
                                break;
                        }
                    }
                    else
                    {
                        sendData();
                    }
                }
                else
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.msgNotConnectionFound, ResourcesRest.MsgBtnOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgException + ex.Message, strMsgOk);
            }
        }

        private void EnabledButtons()
        {
            btnNewDefaultDocument.IsEnabled = true;
            btnDocuments.IsEnabled = true;
            btnCustomer.IsEnabled = true;
            btnProducts.IsEnabled = true;
            btnSync.IsEnabled = true;
            btnSend.IsEnabled = true;
            btnSettings.IsEnabled = true;
            btnLogout.IsEnabled = true;
            btnPortable.IsEnabled = true;
            btnMaps.IsEnabled = true;

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            EnabledButtons();
        }
        protected bool OnBackCliked()
        {
            return true;
        }
        public async void sendData()
        {
            try
            {
                var answer = await DisplayAlert(strMsgTypeUpload, strMsgUpload, strMsgYes, strMsgNo);
                if (answer == true)
                {
                    List<DocumentHeader> SuccessDocumentHeaderList = new List<DocumentHeader>();
                    List<DocumentHeader> FailureDocumentHeaderList = new List<DocumentHeader>();
                    Guid documentStatusToSend = App.SFASettings.DocumentStatusToSendOid;
                    List<DocumentHeader> documentHeaderList = App.DbLayer.GetDocumentHeaders(App.SFASettings.DocumentStatusToSendOid);
                    if (documentHeaderList.Count == 0)
                    {
                        await DisplayAlert(strMsgTypeAlert, strMsgNoDataFound, strMsgOk);
                        return;
                    }

                    int countUploadDocs = 0;
                    int documentsSuccesfullySend = 0;

                    foreach (DocumentHeader CurrentDocumentHeader in documentHeaderList)
                    {
                        countUploadDocs++;
                        DocumentHeader doc = null;
                        try
                        {
                            UserDialogs.Instance.HideLoading();
                            string msg = ResourcesRest.MsgSendingData + " " + countUploadDocs.ToString() + " " + ResourcesRest.From + " " + documentHeaderList.Count().ToString();
                            UserDialogs.Instance.ShowLoading(msg, MaskType.Black);
                            doc = App.DbLayer.LoadDocumentFromDatabase(CurrentDocumentHeader.Oid);

                            if (doc == null)
                            {
                                FailureDocumentHeaderList.Add(doc);
                                continue;
                            }
                            string errorMessage = await DocumentHelper.PrepareSendDocument(doc, doc.DeliveryAddress ?? "");

                            if (string.IsNullOrEmpty(errorMessage))
                            {
                                doc.AddressProfession = doc.BillingAddress?.Profession ?? "";
                                App.DbLayer.UpdateDocumentHeader(doc);
                            }
                            else
                            {
                                FailureDocumentHeaderList.Add(doc);
                                continue;
                            }
                            HttpResponseMessage webresponse = await ApiHelper.SendDocument(doc);
                            if (webresponse == null || (webresponse.StatusCode != HttpStatusCode.Created && webresponse.StatusCode != HttpStatusCode.OK))
                            {
                                await Task.Delay(500);
                                webresponse = await ApiHelper.SendDocument(doc);
                            }
                            ReceivedDocument receivedDocument = await ApiHelper.ConfirmDocumentSend(doc.Oid);
                            if (receivedDocument == null || receivedDocument?.Oid == null || receivedDocument?.Oid == Guid.Empty)
                            {
                                FailureDocumentHeaderList.Add(doc);
                                continue;
                            }
                            doc.IsNewRecord = false;
                            doc.IsSynchronized = true;
                            doc.FiscalDate = DateTime.Now;
                            doc.FinalizedDate = DateTime.Now;
                            if (receivedDocument.Number != -1)
                            {
                                doc.DocumentNumber = receivedDocument.Number;
                            }
                            App.DbLayer.UpdateDocumentHeader(doc);

                            SuccessDocumentHeaderList.Add(doc);
                            documentsSuccesfullySend++;
                        }
                        catch (Exception ex)
                        {
                            if (doc != null)
                            {
                                FailureDocumentHeaderList.Add(doc);
                            }
                            App.LogError(ex);
                            continue;
                        }
                    }
                    string message = string.Empty;

                    if (documentsSuccesfullySend > 0)
                    {
                        message = message + documentsSuccesfullySend + " " + strMsgSuccesfullSend;
                    }
                    if (FailureDocumentHeaderList.Count > 0)
                    {
                        message = message + FailureDocumentHeaderList.Count + " " + strMsgFailure;
                    }
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert(strMsgTypeAlert, message, strMsgOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                UserDialogs.Instance.HideLoading();
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, strMsgOk);
            }
        }

        private static async Task<bool> HasLocationPermissions()
        {
            Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus> permissions = new Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus>();
            var check = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);
            var shouldAsk = await Plugin.Permissions.CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Location);

            if (check != Plugin.Permissions.Abstractions.PermissionStatus.Granted || shouldAsk == true)
            {
                permissions = await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);
            }
            else
            {
                permissions.Add(Plugin.Permissions.Abstractions.Permission.Location, Plugin.Permissions.Abstractions.PermissionStatus.Granted);
            }
            var checkResult = check;
            bool value = permissions.TryGetValue(Plugin.Permissions.Abstractions.Permission.Location, out checkResult);
            if (value == false || check != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                return false;
            }
            return true;
        }

    }
}
