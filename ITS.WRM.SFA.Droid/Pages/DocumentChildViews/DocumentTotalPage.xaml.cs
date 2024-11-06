
using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.DocumentFormat;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentTotalPage : ContentPage
    {
        //private Guid DefaultStatusOid;
        private Guid DefaultCatalogOid;
        private List<DocumentStatus> ListDocumentStatus = new List<DocumentStatus>();
        private List<PriceCatalogPolicy> ListPriceCatalogPolicy = new List<PriceCatalogPolicy>();
        private bool IsShowingFirstTime;
        public List<Address> ListAddress;
        public Guid AddressOid;
        private DateTime _FinalizedDate;
        private DocumentHeader _DocumentHeader = new DocumentHeader();
        private Customer _Customer = new Customer();


        public DocumentTotalPage(DocumentHeader documentHeader, Customer selectedCustomer)
        {
            InitializeComponent();
            InitializeControllers(documentHeader?.DocumentType?.Description);
            _FinalizedDate = documentHeader.FinalizedDate;
            _DocumentHeader = documentHeader;
            _Customer = selectedCustomer;
            DefaultCatalogOid = _DocumentHeader.PriceCatalogPolicyOid;
            LoadDocumentStatus();
            fillData(_DocumentHeader, _Customer);
            IsShowingFirstTime = false;
        }

        private void LoadDocumentStatus()
        {
            int index = 0;
            ListDocumentStatus = App.DocumentStatuses;
            pckDefaultDocumentStatus.SelectedIndex = index;
            pckDefaultDocumentStatus.Items.Clear();
            foreach (DocumentStatus status in ListDocumentStatus)
            {
                if (!pckDefaultDocumentStatus.Items.Contains(status.Description))
                    pckDefaultDocumentStatus.Items.Add(status.Description);
            }
            foreach (DocumentStatus status in ListDocumentStatus)
            {
                if (status.Oid.Equals(_DocumentHeader?.StatusOid ?? Guid.Empty))
                {
                    pckDefaultDocumentStatus.SelectedIndex = index;
                }
                index++;
            }
        }

        public async void OnSave(object sender, EventArgs eventArgs)
        {
            try
            {
                string errorMessage;
                UserDialogs.Instance.ShowLoading("Saving..", MaskType.Black);
                errorMessage = await DocumentHelper.PrepareSaveDocument(_DocumentHeader, txtDeliveryAddress?.Text ?? "");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    BeforeSaveDocument(ref _DocumentHeader);
                    App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.SuccessAdd, ResourcesRest.MsgBtnOk);
                    NavigateAfterSave();
                }
                else
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, errorMessage, ResourcesRest.MsgBtnOk);
                    return;
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

        public async void OnSaveSend(object sender, EventArgs eventArgs)
        {
            try
            {
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.AreYouSureToSendDocument, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (!answer)
                {
                    return;
                }

                var info = App.GetNetworkInfo();
                if (info != null && info.IsConnected == false)
                {
                    var networkAnswqer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.NetworkProblemContinue, ResourcesRest.MsgYes, ResourcesRest.MsgNo);

                    if (!networkAnswqer)
                    {
                        return;
                    }
                }

                bool getLocation = await LocationPermission();
                LocationManager.GetCurrentlocation().ConfigureAwait(false);
                UserDialogs.Instance.ShowLoading("Sending..", MaskType.Black);
                string errorMessage = await DocumentHelper.PrepareSendDocument(_DocumentHeader, txtDeliveryAddress?.Text ?? "");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    BeforeSaveDocument(ref _DocumentHeader);
                    App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + errorMessage, ResourcesRest.MsgBtnOk);
                    });
                    return;
                }
                HttpResponseMessage response = await ApiHelper.SendDocument(_DocumentHeader);
                if (response == null || (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK))
                {
                    await Task.Delay(500);
                    response = await ApiHelper.SendDocument(_DocumentHeader);
                }
                ReceivedDocument receivedDocument = await ApiHelper.ConfirmDocumentSend(_DocumentHeader.Oid);
                if (receivedDocument == null || receivedDocument?.Oid == null || receivedDocument?.Oid == Guid.Empty)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.FailToSend, ResourcesRest.MsgBtnOk);
                    });
                    return;
                }
                _DocumentHeader.IsNewRecord = false;
                _DocumentHeader.IsSynchronized = true;
                _DocumentHeader.FiscalDate = DateTime.Now;
                _DocumentHeader.FinalizedDate = DateTime.Now;
                _DocumentHeader.Username = App.UserName;
                if (receivedDocument.Number != -1)
                {
                    _DocumentHeader.DocumentNumber = receivedDocument.Number;
                }
                App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
                UpdateItemStock(_DocumentHeader.DocumentDetails);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.MsgUpdateDocument, ResourcesRest.MsgBtnOk);
                    NavigateAfterSave();
                });
                UserDialogs.Instance.HideLoading();
                return;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.FailToSend + " " + ex.Message, ResourcesRest.MsgBtnOk);
                    return;
                });
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }


        public async void OnSavePrint(object sender, EventArgs eventArgs)
        {
            try
            {
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.AreYouSureToSendDocument, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (!answer)
                {
                    return;
                }
                var info = App.GetNetworkInfo();
                if (info != null && info.IsConnected == false && _DocumentHeader.IsSynchronized == false)
                {
                    var networkAnswqer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.NetworkProblemContinue, ResourcesRest.MsgYes, ResourcesRest.MsgNo);

                    if (!networkAnswqer)
                    {
                        return;
                    }
                }
                bool getLocation = await LocationPermission();
                LocationManager.GetCurrentlocation().ConfigureAwait(false);

                DependencyService.Get<IBlueTooth>().Cancel();
                bool onlyPrint = true;
                UserDialogs.Instance.ShowLoading("Sending..", MaskType.Black);
                if (!_DocumentHeader.IsSynchronized)
                {
                    onlyPrint = false;
                    string errorMessage = await DocumentHelper.PrepareSendDocument(_DocumentHeader, txtDeliveryAddress?.Text ?? "");
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        BeforeSaveDocument(ref _DocumentHeader);
                        App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
                    }
                    else
                    {
                        await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + errorMessage, ResourcesRest.MsgBtnOk);
                        return;
                    }
                    HttpResponseMessage response = await ApiHelper.SendDocument(_DocumentHeader);
                    if (response == null || (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK))
                    {
                        await Task.Delay(500);
                        response = await ApiHelper.SendDocument(_DocumentHeader);
                    }
                    ReceivedDocument receivedDocument = await ApiHelper.ConfirmDocumentSend(_DocumentHeader.Oid);
                    if (receivedDocument == null || receivedDocument?.Oid == null || receivedDocument?.Oid == Guid.Empty)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            UserDialogs.Instance.HideLoading();
                            DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.FailToSend, ResourcesRest.MsgBtnOk);
                        });
                        return;
                    }
                    _DocumentHeader.IsNewRecord = false;
                    _DocumentHeader.IsSynchronized = true;
                    _DocumentHeader.FiscalDate = DateTime.Now;
                    _DocumentHeader.FinalizedDate = DateTime.Now;
                    _DocumentHeader.Username = App.UserName;
                    if (receivedDocument.Number != -1)
                    {
                        _DocumentHeader.DocumentNumber = receivedDocument.Number;
                    }
                    App.DbLayer.UpdateDocumentHeader(_DocumentHeader);
                    UpdateItemStock(_DocumentHeader.DocumentDetails);
                }
                string failMessage = onlyPrint ? ResourcesRest.DocumentFailToPrint : ResourcesRest.DocumentSentButFailToPrint;
                string successMessage = onlyPrint ? ResourcesRest.DocumentPrintSuccessfully : ResourcesRest.DocumentSentAndPrint;
                bool isPrinted = await Print();
                string resultmessage = isPrinted ? successMessage : failMessage;
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(ResourcesRest.MsgTypeAlert, resultmessage, ResourcesRest.MsgBtnOk);
                });
                if (isPrinted)
                {
                    NavigateAfterSave();
                }
                return;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.FailToSend + " " + ex.Message, ResourcesRest.MsgBtnOk);
                });
                return;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void UpdateItemStock(List<DocumentDetail> details)
        {
            if (_DocumentHeader.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS)
            {
                Dictionary<Guid, double> dict = new Dictionary<Guid, double>();
                foreach (DocumentDetail dtl in details)
                {
                    if (dict.ContainsKey(dtl.ItemOid))
                    {
                        dict[dtl.ItemOid] = dict[dtl.ItemOid] + (double)dtl.Qty;
                    }
                    else
                    {
                        dict.TryAdd(dtl.ItemOid, (double)dtl.Qty);
                    }
                }
                ItemHelper.UpdateStockOnCloseDocument(dict, App.DbLayer, _DocumentHeader.DocumentType);
            }
        }

        private async Task<bool> Print()
        {
            bool isPrinted = false;
            bool _IsConnected = false;
            try
            {
                DependencyService.Get<IBlueTooth>().Cancel();
                if (string.IsNullOrEmpty(App.SFASettings.BlueToothPrinter))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.Fail + "No Configured Device Found On Settings", ResourcesRest.MsgBtnOk);
                    });
                    return false;
                }
                //   await Task.Run(async () =>
                //{
                UserDialogs.Instance.ShowLoading("Connecting To Printer..", MaskType.Black);
                await Task.Delay(1100);
                DependencyService.Get<IBlueTooth>().Start(App.SFASettings.BlueToothPrinter, eBlueToothDevice.PRINTER).ConfigureAwait(false);
                int seconds = 20;
                do
                {
                    seconds--;
                    _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                    await Task.Delay(1000);
                } while (_IsConnected == false && seconds > 0);
                _IsConnected = DependencyService.Get<IBlueTooth>().Connected();
                if (_IsConnected)
                {
                    UserDialogs.Instance.ShowLoading("Printing..", MaskType.Black);
                    await DependencyService.Get<IBlueTooth>().PrintDocument(App.SFASettings.BlueToothPrinter, _DocumentHeader);
                    await Task.Delay(3000);
                    isPrinted = true;
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UserDialogs.Instance.HideLoading();
                        DisplayAlert(ResourcesRest.MsgTypeAlert, "Device Is Not Connected", ResourcesRest.MsgBtnOk);
                    });
                    isPrinted = false;
                }
                // });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return false;
            }
            finally
            {
                DependencyService.Get<IBlueTooth>().Cancel();
            }
            return isPrinted;
        }


        private void BeforeSaveDocument(ref DocumentHeader document)
        {
            document.Remarks = txtRemarks?.Text?.ToString() ?? "";
            document.AddressProfession = document.BillingAddress?.Profession ?? "";
            document.VehicleNumber = txtVehicleNumber.Text;
            User user = null;
            if (App.UserId != null && App.UserId != Guid.Empty)
            {
                user = App.DbLayer.GetById<User>(App.UserId);
            }
            else if (App.UserName != null && !string.IsNullOrEmpty(App.UserName))
            {
                user = App.DbLayer.GetUserByUsername(App.UserName);
            }
            if (user != null)
            {
                if (document.CreatedBy == null || document.CreatedBy == Guid.Empty)
                {
                    document.CreatedBy = user.Oid;
                }
                if (document.UpdatedBy == null || document.UpdatedBy == Guid.Empty)
                {
                    document.UpdatedBy = user.Oid;
                }
            }

            string deviceOid = App.DbLayer.GetAllSfaDevices().Where(x => x.ID == App.SFASettings.SfaId).FirstOrDefault()?.Oid.ToString() ?? "";
            if (document.CreatedByDevice == null || string.IsNullOrEmpty(document.CreatedByDevice))
            {
                document.CreatedByDevice = deviceOid;
            }
            if (document.UpdateByDevice == null || string.IsNullOrEmpty(document.UpdateByDevice))
            {
                document.UpdateByDevice = deviceOid;
            }
        }



        private async Task<bool> LocationPermission()
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

            bool value = permissions.TryGetValue(Plugin.Permissions.Abstractions.Permission.Location, out check);
            return check == Plugin.Permissions.Abstractions.PermissionStatus.Granted ? true : false;

        }

        private void InitializeControllers(string docType)
        {
            ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblCustomer.Text = " " + ResourcesRest.Customer;
            lblStore.Text = " " + ResourcesRest.AddCustomerTxtStore;
            btnSave.Text = ResourcesRest.Save;
            btnSaveSend.Text = ResourcesRest.SaveSend;
            btnSentPrint.Text = _DocumentHeader.IsSynchronized ? ResourcesRest.Print : ResourcesRest.SendPrint;
            lblFinalAmount.Text = " " + ResourcesRest.FinalAmount;
            lblTotalBeforeDiscount.Text = " " + ResourcesRest.TotalAfterDiscount;
            lblTotalDiscount.Text = " " + ResourcesRest.TotalDiscount;
            lblTotalProductsNumber.Text = " " + ResourcesRest.TotalProductsNumber;
            lblTotalVat.Text = " " + ResourcesRest.TotalVATAmount;
            pckDefaultDocumentStatus.Title = " " + ResourcesRest.SettingsPckDocumentStatus;
            lblRemarks.Text = " " + ResourcesRest.TotalOrderLblRemarks;
            lblAddress.Text = " " + ResourcesRest.Address;
            lblDocumentStatus.Text = " " + ResourcesRest.DocumentStatus;
            lblDate.Text = " " + ResourcesRest.Date;
            lblOrderForm.Text = " " + ResourcesRest.TotalOrderHeader;
            lblDocType.Text = (!string.IsNullOrWhiteSpace(docType) ? "( " + docType + " )" : "");
            lblDeliveryAddress.Text = " " + ResourcesRest.DeliveryAddress;
            lblVehicleNumber.Text = ResourcesRest.VehicleNumber;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowingFirstTime)
            {
                fillData(_DocumentHeader, _DocumentHeader.Customer);
            }
        }

        private async void fillData(DocumentHeader documentHeader, Customer selectedCustomer)
        {
            try
            {
                if (selectedCustomer == null)
                {
                    selectedCustomer = App.DbLayer.GetCustomer(documentHeader.CustomerOid);
                }
                Address addr = documentHeader.BillingAddress;
                if (addr == null)
                {
                    addr = App.DbLayer.GetAddressById(documentHeader.BillingAddressOid);
                }
                string address = addr?.City + " " + addr?.Street + " " + addr?.PostCode;

                OwnerApplicationSettings applicationSettings = App.OwnerApplicationSettings;
                Store store = documentHeader?.Store ?? App.Store;
                txtStoreName.Text = store.Name;
                txtTotalDiscount.Text = documentHeader.TotalDiscountAmount.ToString("C" + applicationSettings.DisplayDigits, new CultureInfo("el-GR"));
                txtTotalBeforeDiscount.Text = documentHeader.NetTotalBeforeDiscount.ToString("C" + applicationSettings.DisplayDigits, new CultureInfo("el-GR"));
                txtTotalProductsNumber.Text = documentHeader.TotalQty.ToString();
                txtTotalVat.Text = documentHeader.TotalVatAmount.ToString("C" + applicationSettings.DisplayDigits, new CultureInfo("el-GR"));
                txtFinalAmount.Text = documentHeader.GrossTotal.ToString("C" + applicationSettings.DisplayDigits, new CultureInfo("el-GR"));
                txtDeliveryAddress.Text = string.IsNullOrEmpty(documentHeader.DeliveryAddress) ? address : documentHeader.DeliveryAddress;
                txtRemarks.Text = documentHeader.Remarks;
                txtCustomer.Text = selectedCustomer.CompanyName;
                txtAddress.Text = address;
                txtDeliveryAddress.Text = !string.IsNullOrEmpty(_DocumentHeader.DeliveryAddress) ? _DocumentHeader.DeliveryAddress : address;
                txtVehicleNumber.Text = string.IsNullOrEmpty(txtVehicleNumber.Text) ? App.SFASettings.VehicleNumber : txtVehicleNumber.Text;

            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        void setRemarks(object sender, TextChangedEventArgs e)
        {
            if (_DocumentHeader.Remarks != txtRemarks.Text)
            {
                _DocumentHeader.Remarks = txtRemarks.Text;
            }
        }

        void setDeliveryAddress(object sender, TextChangedEventArgs e)
        {
            if (_DocumentHeader.DeliveryAddress != txtDeliveryAddress.Text)
            {
                Address BillingAddres = _DocumentHeader.BillingAddress ?? App.DbLayer.GetAddressById(_DocumentHeader.BillingAddressOid);
                _DocumentHeader.AddressProfession = BillingAddres?.Profession ?? "";
            }
        }

        void setDocumentStatus(object sender, EventArgs args)
        {
            if (pckDefaultDocumentStatus.SelectedIndex == -1)
            {
                DocumentStatus status = ListDocumentStatus.Where(x => x.Oid == App.SFASettings.DefaultDocumentStatusOid).FirstOrDefault();
                _DocumentHeader.Status = status;
                _DocumentHeader.StatusOid = status.Oid;
            }
            else
            {
                string DefaultOrder = pckDefaultDocumentStatus.Items[pckDefaultDocumentStatus.SelectedIndex];
                DocumentStatus newStatus = ListDocumentStatus.Where(x => x.Description == DefaultOrder).FirstOrDefault();
                _DocumentHeader.Status = newStatus;
                _DocumentHeader.StatusOid = newStatus.Oid;
            }
            if (_DocumentHeader.StatusOid != App.SFASettings.DocumentStatusToSendOid)
            {
                pckDefaultDocumentStatus.TextColor = Color.FromHex("#f30303");
            }
            else
            {
                pckDefaultDocumentStatus.TextColor = Color.FromHex("#000000");
            }
        }

        public void DeliveryAddressFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void DeliveryAddressUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void RemarksFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void RemarksUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
        public void VehicleNumberFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }
        public void VehicleNumberUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }


        private void NavigateAfterSave()
        {
            IReadOnlyList<Page> stack = Navigation.NavigationStack;
            List<Page> pagesToRemove = new List<Page>();
            foreach (Page page in stack)
            {
                if (page.GetType() == typeof(DocumentCustomerPage) || page.GetType() == typeof(DocumentTabPage))
                {
                    pagesToRemove.Add(page);
                }
            }
            foreach (Page page in pagesToRemove)
            {
                if (Navigation.NavigationStack.Contains(page))
                {
                    Navigation.RemovePage(page);
                }
            }
        }

    }
}