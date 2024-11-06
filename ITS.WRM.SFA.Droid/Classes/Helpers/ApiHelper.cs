using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public static class ApiHelper
    {
        public static async Task<ReceivedDocument> ConfirmDocumentSend(Guid Oid)
        {
            string content = string.Empty;
            ReceivedDocument receivedDocument = null;
            HttpRequests client = new HttpRequests();
            try
            {
                string url = string.Format(@"/Custom/ConfirmReceivedDocument/" + Oid.ToString());
                content = await client.Get(url);
                if (!content.Contains("error") && !string.IsNullOrEmpty(content) && content != "[]")
                {
                    receivedDocument = JsonConvert.DeserializeObject<ReceivedDocument>(content);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                receivedDocument = null;
            }
            finally
            {
                client.Dispose();
            }
            return receivedDocument;
        }

        public static async Task<HttpResponseMessage> SendDocument(DocumentHeader header)
        {
            HttpResponseMessage response = null;
            HttpRequests client = new HttpRequests(App.SFASettings.ApiTimeout);
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                response = await client.Post("/DocumentHeader", header);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return response;
        }

        public static async Task<HttpResponseMessage> Authenticate(string username, string password)
        {
            HttpResponseMessage response = null;
            HttpRequests client = new HttpRequests(App.SFASettings.ApiTimeout);
            try
            {
                response = await client.ServerAuthenticate(username, password);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return response;
        }


        public static async Task<HttpResponseMessage> PostCustomer(Customer obj)
        {
            HttpResponseMessage response = null;
            HttpRequests client = new HttpRequests(App.SFASettings.ApiTimeout);
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                response = await client.Post("/Customer", obj);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return response;
        }

        public static async Task<HttpResponseMessage> EditCustomer(Customer obj)
        {
            HttpResponseMessage response = null;
            HttpRequests client = new HttpRequests(App.SFASettings.ApiTimeout);
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                response = await client.Put(string.Format("/Customer({0})", obj.Oid), obj);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return response;
        }

        public static async Task<HttpResponseMessage> PostAddress(Address obj)
        {
            HttpResponseMessage response = null;
            HttpRequests client = new HttpRequests();
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                response = await client.Post("/Address", obj);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return response;
        }

        public static async Task<HttpResponseMessage> EditAddress(Address obj)
        {
            HttpResponseMessage response = null;
            HttpRequests client = new HttpRequests();
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                response = await client.Put(string.Format("/Address({0})", obj.Oid), obj);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return response;
        }



        public static async Task<Customer> GetCustomer(Guid Oid)
        {
            Customer obj = new Customer();
            HttpRequests client = new HttpRequests();
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                string response = await client.Get(@"/Customer(" + Oid.ToString() + ")/?$expand=Trader");
                obj = JsonConvert.DeserializeObject<Customer>(response);
            }
            catch (Exception ex)
            {
                obj = null;
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return obj;
        }


        public static async Task<Trader> GetTraderByTaxCode(String TaxCode)
        {
            HttpRequests client = new HttpRequests();
            try
            {
                await client.ServerAuthenticate(App.UserName, App.UserPassword);
                string response = await client.Get("/Trader?$filter=TaxCode eq '" + TaxCode + "'&$expand=Addresses");
                List<Trader> objList = JsonConvert.DeserializeObject<List<Trader>>(JObject.Parse(response)["value"].ToString());
                if (objList != null && objList.Count > 0)
                {
                    return objList.First();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                client.Dispose();
            }
            return null;
        }

    }
}