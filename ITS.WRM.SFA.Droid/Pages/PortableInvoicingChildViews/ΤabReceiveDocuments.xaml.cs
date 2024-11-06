using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Model.Model;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using System.Globalization;

namespace ITS.WRM.SFA.Droid.Pages.PortableInvoicingChildViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ΤabReceiveDocuments : ContentPage
    {

        private List<StockDocumentHeader> documents = new List<StockDocumentHeader>();
        private List<StockDetail> Details = new List<StockDetail>();
        private Guid SelectdedDocument = Guid.Empty;
        private DateTime DateFrom = DateTime.Now;
        private DateTime DateTo = DateTime.Now;

        public ΤabReceiveDocuments()
        {
            InitializeComponent();
            InitiallizeControllers();
            Task.Run(async () =>
            {
                await UpdateViewList();
            });
            pckDateFrom.DateSelected += (sender, args) =>
            {
                DateFrom = args.NewDate;
                Task.Run(async () =>
                {
                    await UpdateViewList();
                });
            };
            pckDateTo.DateSelected += (sender, args) =>
            {
                DateTo = args.NewDate;
                Task.Run(async () =>
                {
                    await UpdateViewList();
                });
            };

        }

        private async Task UpdateViewList()
        {
            UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
            await Task.Run(async () =>
             {
                 documents = App.DbLayer.GetStockDocuments(DateFrom, DateTo);
                 await Task.Delay(500);
             });
            Device.BeginInvokeOnMainThread(() =>
            {
                if (documents != null && documents.Count > 0)
                {
                    DocumentList.ItemsSource = documents;
                    if (SelectdedDocument == Guid.Empty)
                    {
                        Details = new List<StockDetail>();
                        DetailList.ItemsSource = Details;
                    }
                    else
                    {
                        Details = App.DbLayer.GetStockDocumentById(SelectdedDocument).Details;
                        DetailList.ItemsSource = Details;
                    }
                }
                else
                {
                    DocumentList.ItemsSource = new List<StockDocumentHeader>();
                    Details = new List<StockDetail>();
                    DetailList.ItemsSource = Details;
                }
                UserDialogs.Instance.HideLoading();
            });
        }

        private async Task<int> GetDocumentsFromServer(long fromDate, long toDate)
        {
            string content = string.Empty;
            int headersCount = 0;
            long maxVersion = App.DbLayer.GetTableVersionByName("StockDocumentHeader")?.Version ?? 0;
            ITS.WRM.SFA.Model.Model.SFA sfa = App.DbLayer.GetAllSfaDevices().Where(x => x.ID == App.SFASettings.SfaId).FirstOrDefault();
            if (sfa == null)
            {
                throw new Exception("WRONG SETTINGS SFA DEVICE NOT FOUND");
            }
            using (HttpRequests client = new HttpRequests(App.SFASettings.ApiTimeout))
            {
                string url = @"/Custom/GetStockInitialiseDocuments/" + sfa.Oid.ToString() + "/" + fromDate + "/" + toDate + "/" + App.Store?.Oid.ToString() + "/" + maxVersion;
                content = await client.Get(url);
            }
            if (content.Contains("error"))
            {
                Microsoft.AppCenter.Crashes.Crashes.TrackError(new Exception(content));
                throw new Exception(ResourcesRest.Fail);
            }
            if (!string.IsNullOrEmpty(content) && content != "[]")
            {
                List<StockDocumentHeader> stockHeaders = JsonConvert.DeserializeObject<List<StockDocumentHeader>>(content).OrderBy(x => x.CreatedOnTicks).ToList();
                if (stockHeaders != null && stockHeaders.Count > 0)
                {
                    foreach (StockDocumentHeader sh in stockHeaders)
                    {
                        try
                        {
                            StockDocumentHeader existing = App.DbLayer.GetById<StockDocumentHeader>(sh.Oid);
                            if (existing == null && sh.UpdatedOnTicks > maxVersion)
                            {
                                App.DbLayer.InsertOrReplaceObjWitoutStamp<StockDocumentHeader>(sh);
                                sh.Details.ForEach(x => App.DbLayer.InsertOrReplaceObj<StockDetail>(x));
                                headersCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
                            RollBack(sh, sh.Details);
                        }
                    }
                }
            }
            return headersCount;
        }

        private void ExecuteStockDocument(Guid Oid)
        {
            StockDocumentHeader header = App.DbLayer.GetStockDocumentById(Oid);
            if (header != null)
            {
                if (header.Executed)
                {
                    throw new Exception(ResourcesRest.DocumentAlreadyExecuted);
                }
                foreach (StockDetail dtl in header.Details)
                {
                    Item item = App.DbLayer.GetItem(dtl.ItemOid);
                    if (item != null)
                    {
                        double stock;
                        if (dtl.Qty.ToString().TryParse(out stock))
                        {
                            item.Stock = item.Stock + stock;
                            App.DbLayer.InsertOrReplaceObj<Item>(item);
                        }
                    }
                }
                header.Executed = true;
                App.DbLayer.InsertOrReplaceObjWitoutStamp<StockDocumentHeader>(header);
                TableVersion tv = App.DbLayer.GetTableVersionByName("StockDocumentHeader");
                if (tv == null)
                {
                    App.DbLayer.CreateTableVersionRow(typeof(StockDocumentHeader), header.UpdatedOnTicks);
                }
                else
                {
                    tv.Version = header.UpdatedOnTicks;
                    App.DbLayer.Update(tv, typeof(TableVersion));
                }
            }
        }

        private void RollBack(StockDocumentHeader header, List<StockDetail> dtls)
        {
            try
            {
                bool updateTableVersion = false;
                if (header != null)
                {
                    updateTableVersion = header.Executed ? false : true;
                    if (dtls != null && dtls.Count > 0)
                    {
                        dtls.ForEach(x => App.DbLayer.DeleteObj<StockDetail>(x));
                    }
                }
                App.DbLayer.DeleteObj<StockDocumentHeader>(header);
                if (updateTableVersion)
                {
                    long max = App.DbLayer.GetMaxUpdatedOnTicksFromTable(typeof(StockDocumentHeader));
                    TableVersion tv = App.DbLayer.GetTableVersionByName("StockDocumentHeader");
                    if (tv == null)
                    {
                        App.DbLayer.CreateTableVersionRow(typeof(StockDocumentHeader), max);
                    }
                    else
                    {
                        tv.Version = max;
                        App.DbLayer.Update(tv, typeof(TableVersion));
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        protected async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                ListView listView = sender as ListView;
                StockDocumentHeader sdh = (StockDocumentHeader)listView.SelectedItem;
                if (sdh != null)
                {
                    SelectdedDocument = sdh.Oid;
                }

                if (SelectdedDocument == Guid.Empty)
                {
                    Details = new List<StockDetail>();
                    DetailList.ItemsSource = Details;
                }
                else
                {
                    Details = App.DbLayer.GetStockDocumentById(SelectdedDocument).Details;
                    DetailList.ItemsSource = Details;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        protected async void OnGetStockDocuments(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                string errorMessage;
                if (!CheckRequiredLoadingSettings(out errorMessage))
                {
                    UserDialogs.Instance.HideLoading();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(ResourcesRest.MsgTypeSync, errorMessage, ResourcesRest.MsgBtnOk);
                        return;
                    });
                    return;
                }

                int count = 0;
                long fromDateTicks = DateFrom.Date.Ticks;
                long toDateTicks = DateTo.Date.AddHours(23).AddMinutes(59).Ticks;
                count = await GetDocumentsFromServer(fromDateTicks, toDateTicks);
                //await Task.Run(async () =>
                //{
                //    long fromDateTicks = DateFrom.Date.Ticks;
                //    long toDateTicks = DateTo.Date.AddHours(23).AddMinutes(59).Ticks;
                //    count = await GetDocumentsFromServer(fromDateTicks, toDateTicks);
                //});
                await UpdateViewList();
                UserDialogs.Instance.HideLoading();
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeSync, count.ToString() + " " + ResourcesRest.Documents + " " + ResourcesRest.Received, ResourcesRest.MsgBtnOk);
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                });
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void DeleteDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.msgConfirmDelete, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (answer == true)
                {
                    Guid HeaderOid = (Guid)menuItem.CommandParameter;
                    StockDocumentHeader currentDocument = App.DbLayer.GetStockDocumentById(HeaderOid);
                    RollBack(currentDocument, currentDocument.Details);
                    SelectdedDocument = Guid.Empty;
                    Details = new List<StockDetail>();
                    DetailList.ItemsSource = Details;
                    await UpdateViewList();
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.SuccessAdd, ResourcesRest.MsgBtnOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        protected async void ExecuteDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.UpdateStockFromDocument, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (answer == true)
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                    Guid HeaderOid = (Guid)menuItem.CommandParameter;
                    ExecuteStockDocument(HeaderOid);
                }
                await UpdateViewList();
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.SuccessAdd, ResourcesRest.MsgBtnOk);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private bool CheckRequiredLoadingSettings(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (App.SFASettings.LoadingDocumentTypeOid == null || App.SFASettings.LoadingDocumentTypeOid == Guid.Empty)
            {

                errorMessage = ResourcesRest.WrongSettingsLoadingDocument;
                return false;
            }
            var sfa = App.DbLayer.GetAllSfaDevices().Where(x => x.ID == App.SFASettings.SfaId).FirstOrDefault();
            if (sfa == null)
            {
                errorMessage = ResourcesRest.WrongSettingsLoadingDocument;
                return false;
            }
            return true;
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblDocuments.Text = ResourcesRest.StockDocuments;
            lblFrom.Text = ResourcesRest.From;
            lblTo.Text = ResourcesRest.To;
            btnGetStockDocuments.Text = ResourcesRest.GetStockDocuments;
            lblCreatedDate.Text = ResourcesRest.CreatedDate;
            lblStatus.Text = ResourcesRest.DocumentStatus;
            lblTotalQty.Text = ResourcesRest.Qty;
            lblDocumentDescription.Text = ResourcesRest.Description;
            lblDetailDescription.Text = ResourcesRest.Description;
            lblDetailCode.Text = ResourcesRest.Code;
            lblDetailQty.Text = ResourcesRest.Qty;
        }
    }
}