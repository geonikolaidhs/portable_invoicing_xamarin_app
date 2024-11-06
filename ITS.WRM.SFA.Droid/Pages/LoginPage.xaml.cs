using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLCrypto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {

        private string strMsgBtnOk;
        private string strMsgTypeSave;
        private string strMsgFailureBadRequestLogin;
        private string strMsgFailureLoginUrl;
        private string strMsgTypeLoginFail;
        private string strAlert;
        private string strMsgFailure;
        private string strMsgServiceUnavailable;

        public LoginPage()
        {
            InitializeComponent();
            InitiallizeControllers();
            App.Login = false;
#if DEBUG
            Username.Text = "tabtest";
            //Username.Text = "admin";
            password.Text = "1234";

            //Username.Text = "admin";
            //password.Text = "1t$erviceS2015";
#endif
        }

        async void OnButtonClickedLogin(object sender, EventArgs eventArguments)
        {
            if (string.IsNullOrEmpty(password.Text) || string.IsNullOrEmpty(Username.Text))
            {
                await DisplayAlert("Error", ResourcesRest.PleaseFillInAllRequiredFields, strMsgBtnOk);
                return;
            }
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoginPleaseWait, MaskType.Black);
            await Task.Run(async () =>
            {
                await Authenticate(Username.Text, password.Text);

            });
            UserDialogs.Instance.HideLoading();
        }

        private static bool toMainPage = false;
        async Task Authenticate(string username, string pass)
        {
            try
            {
                toMainPage = false;
                string encodedPassword = EncodePassword(password.Text);
                User CurrentUser = App.DbLayer.CheckUser(Username.Text, encodedPassword);
                AndroidMethods am = new AndroidMethods();
                string settinsError = string.Empty;
                if (CurrentUser != null)
                {
                    //SaveCredentials(username, pass, CurrentUser.Oid);
                    App.UserName = username;
                    App.UserPassword = pass;
                    App.UserId = CurrentUser.Oid;
                    App.Login = true;
                    await Task.Delay(1);
                    if (!am.ValidateSettings(App.SFASettings, out settinsError))
                    {
                        Application.Current.MainPage = new NavigationPage(new SettingsTabPage("Login"));
                        UserDialogs.Instance.HideLoading();
                        return;
                    }
                    CheckUserOnLine(username, pass).ConfigureAwait(false);
                    if (!toMainPage)
                    {
                        toMainPage = true;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage = new NavigationPage(new MainPage());
                            UserDialogs.Instance.HideLoading();
                        });
                    }
                }
                else
                {
                    App.Login = false;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(strMsgTypeLoginFail, strMsgTypeLoginFail, strMsgBtnOk);
                        UserDialogs.Instance.HideLoading();
                    });
                    return;

                    //List<User> users = App.DbLayer.GetUsers();
                    //if (users.Count > 0)
                    //{
                    //    Device.BeginInvokeOnMainThread(() =>
                    //    {
                    //        DisplayAlert(strMsgTypeLoginFail, strMsgTypeLoginFail, strMsgBtnOk);
                    //        UserDialogs.Instance.HideLoading();
                    //    });
                    //    return;
                    //}
                    //else
                    //{
                    //    SaveCredentials(Username.Text, password.Text, Guid.Empty);
                    //    await CheckUserOnLine();
                    //}
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                if (!App.Login)
                {
                    Device.BeginInvokeOnMainThread(() =>
                      {
                          DisplayAlert(strMsgTypeLoginFail, strMsgServiceUnavailable, strMsgBtnOk);
                          UserDialogs.Instance.HideLoading();
                      });
                }

            }
        }

        async Task CheckUserOnLine(string username, string password)
        {
            try
            {
                if (DependencyService.Get<ICrossPlatformMethods>().IsConnected())
                {
                    HttpResponseMessage response = await ApiHelper.Authenticate(username, password);
                    if (response == null || response.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                    {
                        if (!App.Login)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert(strAlert, ResourcesRest.msgNotConnectionFound, strMsgBtnOk);
                            });
                            return;
                        }
                    }
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            if (!App.Login)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(strMsgTypeLoginFail, strMsgFailureBadRequestLogin, strMsgBtnOk);
                                });
                            }
                            break;
                        case System.Net.HttpStatusCode.ServiceUnavailable:
                            if (!App.Login)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(strAlert, strMsgServiceUnavailable, strMsgBtnOk);
                                });
                            }
                            break;
                        case System.Net.HttpStatusCode.OK:
                            // int result = await SuccessAuthedicate();
                            //if (!App.Login && result == 0)
                            //{
                            //    Device.BeginInvokeOnMainThread(() =>
                            //    {
                            //        if (!App.Login)
                            //            DisplayAlert(strMsgTypeLoginFail, strMsgFailureBadRequestLogin, strMsgBtnOk);
                            //    });
                            //}
                            break;
                        default:
                            if (!App.Login)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert(strMsgTypeLoginFail, strMsgServiceUnavailable, strMsgBtnOk);
                                });
                            }
                            break;
                    }
                }
                else
                {
                    if (!App.Login)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(strAlert, ResourcesRest.msgNotConnectionFound, strMsgBtnOk);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                if (!App.Login)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(strMsgTypeLoginFail, ex.Message, strMsgBtnOk);
                        UserDialogs.Instance.HideLoading();
                    });
                }
            }
        }

        //async Task<int> SuccessAuthedicate(Guid userOid)
        //{
        //    try
        //    {
        //        HttpRequests httpRequest = new HttpRequests();
        //        SaveCredentials(Username.Text, password.Text, userOid);
        //        string content = await httpRequest.Get("/User?$filter=UserName eq '" + Username.Text + "'");
        //        List<User> CurrentUser = JsonConvert.DeserializeObject<List<User>>(JObject.Parse(content)["value"].ToString());
        //        if (CurrentUser != null && CurrentUser.FirstOrDefault() != null)
        //        {
        //            SaveCredentials(Username.Text, password.Text, CurrentUser.FirstOrDefault().Oid);
        //            if (!App.Login)
        //            {
        //                App.Login = true;
        //                Device.BeginInvokeOnMainThread(() =>
        //                {
        //                    Application.Current.MainPage = new NavigationPage(new MainPage());
        //                });
        //                return 1;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.LogError(ex);
        //    }
        //    return 0;
        //}

        //public static void SaveCredentials(string userName, string password, Guid UserId)
        //{
        //    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        //    {
        //        Account account = new Account
        //        {
        //            Username = userName
        //        };
        //        account.Properties.Add("Password", password);
        //        AccountStore.Create().Save(account, "SFA");

        //        App.UserName = userName;
        //        App.UserPassword = password;
        //        App.UserId = UserId;
        //    }
        //}
        //public static void DeleteCredentials()
        //{
        //    Account account = AccountStore.Create().FindAccountsForService("").FirstOrDefault();
        //    if (account != null)
        //    {
        //        AccountStore.Create().Delete(account, "SFA");
        //        App.UserId = Guid.Empty;
        //        App.UserName = "";
        //    }
        //}



        public string EncodePassword(string originalPassword)
        {
            //Declarations
            byte[] originalBytes;
            byte[] encodedBytes;
            byte[] doubleEncodedBytes;
            IHashAlgorithmProvider md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            //First encoding using MD5 Algorithm
            md5 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5);
            originalBytes = Encoding.UTF8.GetBytes(originalPassword);
            encodedBytes = md5.HashData(originalBytes);
            //Second encoding using SHA1 Algorithm
            IHashAlgorithmProvider sha1 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1);
            doubleEncodedBytes = sha1.HashData(encodedBytes);
            return BitConverter.ToString(doubleEncodedBytes);
        }

        async public void OnSettings(object sender, EventArgs eventArgs)
        {
            IReadOnlyList<Page> stack = Navigation.NavigationStack;
            if (stack[stack.Count - 1].GetType() != typeof(SettingsTabPage))
            {
                await Navigation.PushAsync(new SettingsTabPage("Login"));
            }
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            Username.Placeholder = ResourcesRest.LoginUserName;
            password.Placeholder = ResourcesRest.LoginPassword;
            btnLogin.Text = ResourcesRest.LoginBtnLogin;
            strMsgBtnOk = ResourcesRest.MsgBtnOk;
            strMsgTypeSave = ResourcesRest.Save;
            strMsgFailureBadRequestLogin = ResourcesRest.LoginMsgFailureLoginBadRequest;
            strMsgFailureLoginUrl = ResourcesRest.LoginMsgFailureLoginUrl;
            strMsgTypeLoginFail = ResourcesRest.MsgTypeLoginFailed;
            strAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgServiceUnavailable = ResourcesRest.strMsgServiceUnavailable;
            btnSettings.Text = ResourcesRest.SettingsLblSettings;
        }

        public void UsernameFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void UsernameUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void PasswordFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void PasswordUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }


    }
}
