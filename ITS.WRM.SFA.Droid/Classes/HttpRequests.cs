using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.NonPersistant;
using Microsoft.Win32.SafeHandles;
using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace ITS.WRM.SFA.Droid.Classes
{
    public class HttpRequests : IDisposable
    {
        public HttpRequests()
        {
            this.Timeout = App.SFASettings.ApiTimeout;
        }

        public HttpRequests(int timeout)
        {
            this.Timeout = timeout;
        }

        private int Timeout;

        private async Task<HttpResponseMessage> GetToken()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.NotModified;
            try
            {
                if (App.token == null || DateTime.Now >= Convert.ToDateTime(App.token.expires))
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    //Account account = (AccountStore.Create().FindAccountsForService("SFA") as List<Account>).FirstOrDefault();
                    response = await ServerAuthenticate(App.UserName, App.UserPassword);
                    if (App.token == null || DateTime.Now >= Convert.ToDateTime(App.token.expires))
                    {
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return response;
        }



        // Get
        public async Task<string> Get(string path)
        {
            string content = string.Empty;
            try
            {
                var tokenResponse = await GetToken();
                if (tokenResponse.StatusCode == HttpStatusCode.NotModified || tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (HttpClient httpClient = new HttpClient(new NativeMessageHandler()))
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(this.Timeout);
                        httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + App.token.access_token);
                        HttpResponseMessage response = await httpClient.GetAsync(App.SFASettings.ServerURL + path);
                        content = await response.Content.ReadAsStringAsync();
                    }
                }
                return content;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return "error : " + ex.Message;
            }
        }

        // Put
        public async Task<HttpResponseMessage> Put(string Path, BasicObj obj)
        {
            HttpResponseMessage response = null;
            try
            {
                HttpResponseMessage tokenResponse = await GetToken();
                if (tokenResponse.StatusCode == HttpStatusCode.NotModified || tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (HttpClient httpClient = new HttpClient(new NativeMessageHandler()))
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(this.Timeout);
                        httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + App.token.access_token);
                        string str = JsonConvert.SerializeObject(obj);
                        StringContent Body = new StringContent(str);
                        response = await httpClient.PutAsync(new Uri(App.SFASettings.ServerURL + Path), Body);
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            return response;
        }

        //Post
        public async Task<HttpResponseMessage> Post(string Path, BasicObj obj)
        {
            try
            {
                HttpResponseMessage tokenResponse = await GetToken();
                if (tokenResponse.StatusCode == HttpStatusCode.NotModified || tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (HttpClient httpClient = new HttpClient(new NativeMessageHandler()))
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(this.Timeout);
                        httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + App.token.access_token);
                        string jsonSerialiseResult = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
                        StringContent Body = new StringContent(jsonSerialiseResult);
                        HttpResponseMessage response = await httpClient.PostAsync(new Uri(App.SFASettings.ServerURL + Path), Body);
                        return response;
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private async Task<bool> CheckUnAuthorizedResponse(HttpResponseMessage response)
        {
            bool isUnAuthorized = false;
            if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                isUnAuthorized = true;
                await ServerAuthenticate(App.UserName, App.UserPassword);
            }
            return isUnAuthorized;
        }

        //Post Authentication Request
        public async Task<HttpResponseMessage> ServerAuthenticate(string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(Timeout);
                    var contentToSend = CreateAuthenticationContent(username, password);
                    Uri uri = new Uri(App.SFASettings.AuthenticationURL);
                    var response = await client.PostAsync(uri, contentToSend);
                    var responeContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        App.token = JsonConvert.DeserializeObject<Token>(responeContent);
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            }
        }

        private FormUrlEncodedContent CreateAuthenticationContent(string username, string password)
        {
            return new FormUrlEncodedContent(new[]{
                            new KeyValuePair<string, string>("grant_type", "password"),
                            new KeyValuePair<string, string>("username", username),
                            new KeyValuePair<string, string>("password", password)
                     });

        }


        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();

            }
            disposed = true;
        }

    }
}
