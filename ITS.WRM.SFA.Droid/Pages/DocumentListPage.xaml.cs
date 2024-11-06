using Acr.UserDialogs;
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentListPage : ContentPage
    {
        ObservableCollection<OrderPresent> ObservableOrdersPresent = new ObservableCollection<OrderPresent>();
        private List<DocumentStatus> ListDocumentStatus = new List<DocumentStatus>();
        private List<DocumentType> ListDocumentType = new List<DocumentType>();
        private bool IsShowingFirstTime = true;
        private Guid DocumentStatusOid;
        private Guid DocumentTypeOid;
        private DateTime DateFrom = DateTime.Now;
        private DateTime DateTo = DateTime.Now;
        private enumSearchOrder.Criteria searchCriteria;

        public DocumentListPage()
        {
            InitializeComponent();
            InitiallizeControllers();
            LoadDocumentStatus();
            LoadDocumenTypes();
            IsShowingFirstTime = true;
            searchCriteria = enumSearchOrder.Criteria.All;
            DocumentStatusOid = Guid.Empty;
            DocumentTypeOid = Guid.Empty;

            pckDocumentStatus.SelectedIndexChanged += (sender, args) =>
            {
                if (pckDocumentStatus.SelectedIndex == -1)
                {
                    DocumentStatusOid = Guid.Empty;
                }
                else
                {
                    string StatusName = pckDocumentStatus.Items[pckDocumentStatus.SelectedIndex];
                    DocumentStatusOid = ListDocumentStatus.Find(x => x.Description.Equals(StatusName))?.Oid ?? Guid.Empty;
                }
            };
            pckDocumentType.SelectedIndexChanged += (sender, args) =>
            {
                if (pckDocumentType.SelectedIndex == -1)
                {
                    DocumentTypeOid = Guid.Empty;
                }
                else
                {
                    string TypeName = pckDocumentType.Items[pckDocumentType.SelectedIndex];
                    bool found = false;
                    foreach (DocumentType type in ListDocumentType)
                    {
                        if (type.Description == TypeName)
                        {
                            DocumentTypeOid = type.Oid;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        DocumentTypeOid = Guid.Empty;
                }
            };
            pckDateFrom.DateSelected += (sender, args) =>
            {
                DateFrom = args.NewDate;
            };
            pckDateTo.DateSelected += (sender, args) =>
            {
                DateTo = args.NewDate;
            };
            switchSearchType.Toggled += SetSearchMode;
        }

        void SetSearchMode(object sender, ToggledEventArgs e)
        {
            string status = e.Value.ToString();
            if (e.Value == true)
            {
                searchCriteria = enumSearchOrder.Criteria.All;
            }
            else
            {
                searchCriteria = enumSearchOrder.Criteria.Specific;
            }
        }

        protected async Task LoadOrders(Guid statusOid, Guid TypeOid, string filter, DateTime From, DateTime To, enumSearchOrder.Criteria searchType)
        {
            try
            {
                if (filter != null && filter != "")
                {
                    filter = filter.ToUpper();
                }
                ObservableOrdersPresent.Clear();
                List<OrderPresent> ListOrders = new List<OrderPresent>();
                ListOrders = App.DbLayer.GetAllOrders(statusOid, TypeOid, filter, From, To, searchType);
                if (ListOrders.Count <= 0)
                {
                    ObservableOrdersPresent.Add(new OrderPresent() { CompanyName = ResourcesRest.NoResultsFoundMessage });
                }
                else
                {
                    ObservableOrdersPresent = new ObservableCollection<OrderPresent>(ListOrders);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    OrderList.ItemsSource = ObservableOrdersPresent.OrderBy(x => x.Status);
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        protected async void EditDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                Guid HeaderOid = ObservableOrdersPresent.Where(headerOid => headerOid.Oid.Equals(menuItem.CommandParameter)).FirstOrDefault()?.Oid ?? Guid.Empty;
                if (HeaderOid == null || HeaderOid == Guid.Empty)
                {
                    return;
                }
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    DocumentHeader selectedHeader = App.DbLayer.LoadDocumentFromDatabase(HeaderOid);
                    if (selectedHeader != null)
                    {
                        IReadOnlyList<Page> stack = Navigation.NavigationStack;
                        if (stack[stack.Count - 1].GetType() != typeof(DocumentTabPage))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushAsync(new DocumentTabPage(selectedHeader));
                            });
                        }
                        UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        throw new Exception(ResourcesRest.NoResultsFoundMessage);
                    }
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void PrintDocumentHeader(object sender, EventArgs e)
        {

            bool _IsConnected = false;
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                Guid HeaderOid = ObservableOrdersPresent.Where(headerOid => headerOid.Oid.Equals(menuItem.CommandParameter)).FirstOrDefault()?.Oid ?? Guid.Empty;
                if (HeaderOid == null || HeaderOid == Guid.Empty)
                {
                    return;
                }
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(async () =>
                {
                    if (string.IsNullOrEmpty(App.SFASettings.BlueToothPrinter))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(ResourcesRest.MsgTypeAlert, "No Configured Device Found On Settings", ResourcesRest.MsgBtnOk);
                            UserDialogs.Instance.HideLoading();
                        });
                        return;
                    }

                    DocumentHeader selectedHeader = App.DbLayer.LoadDocumentFromDatabase(HeaderOid);
                    if (selectedHeader != null)
                    {
                        if (!selectedHeader.IsSynchronized)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.SentDocumentBeforePrinting, ResourcesRest.MsgBtnOk);
                                UserDialogs.Instance.HideLoading();
                            });
                            return;
                        }

                        DependencyService.Get<IBlueTooth>().Cancel();
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
                            await DependencyService.Get<IBlueTooth>().PrintDocument(App.SFASettings.BlueToothPrinter, selectedHeader);
                            await Task.Delay(3000);

                            UserDialogs.Instance.HideLoading();
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.DocumentPrintSuccessfully, ResourcesRest.MsgBtnOk);
                                UserDialogs.Instance.HideLoading();
                            });
                            return;
                        }
                        else
                        {
                            UserDialogs.Instance.HideLoading();
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                UserDialogs.Instance.HideLoading();
                                DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.DeviceNotConnected, ResourcesRest.MsgBtnOk);
                            });
                            return;
                        }
                    }
                    else
                    {
                        throw new Exception(ResourcesRest.NoResultsFoundMessage);
                    }
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.DocumentFailToPrint + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
                return;
            }
        }




        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblOrderList.Text = ResourcesRest.OrderList;
            lblSelectedDocumentStatus.Text = ResourcesRest.DocumentStatus;
            pckDocumentStatus.Title = ResourcesRest.SelectOrderStatus;
            pckDocumentType.Title = ResourcesRest.SelectDocumentType;
            srchDescription.Placeholder = ResourcesRest.SearchByNameOrTaxCode;
            lblCustomer.Text = ResourcesRest.Customer;
            lblFrom.Text = ResourcesRest.From;
            lblTo.Text = ResourcesRest.To;
            lblSearchType.Text = ResourcesRest.AllDocuments;
            lblSelectedDocumentType.Text = ResourcesRest.DocumentType;
            Customer.Text = ResourcesRest.DocumentCustomer;
            CreatedDate.Text = ResourcesRest.CreatedDate;
            Status.Text = ResourcesRest.DocumentStatus;
            Type.Text = ResourcesRest.Type;
            GrossTotal.Text = ResourcesRest.GrossTotal;
            IsSynchronized.Text = ResourcesRest.IsSynchronized;
        }

        private async void LoadDocumentStatus()
        {
            try
            {
                ListDocumentStatus = App.DocumentStatuses;
                DocumentStatusOid = App.SFASettings.DefaultDocumentStatusOid;
                int index = 0;
                pckDocumentStatus.Items.Clear();
                foreach (DocumentStatus status in ListDocumentStatus)
                {
                    if (!pckDocumentStatus.Items.Contains(status.Description))
                    {
                        pckDocumentStatus.Items.Add(status.Description);
                    }
                    index++;
                }
                if (!pckDocumentStatus.Items.Contains(ResourcesRest.All))
                {
                    pckDocumentStatus.Items.Add(ResourcesRest.All);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        private async void LoadDocumenTypes()
        {
            try
            {
                ListDocumentType = App.DbLayer.GetAllValidDocumentTypes();
                DocumentTypeOid = App.SFASettings.DefaultDocumentTypeOid;
                int index = 0;
                pckDocumentType.Items.Clear();
                foreach (DocumentType type in ListDocumentType)
                {
                    if (!pckDocumentType.Items.Contains(type.Description))
                    {
                        pckDocumentType.Items.Add(type.Description);
                    }
                    index++;
                }
                if (!pckDocumentType.Items.Contains(ResourcesRest.All))
                {
                    pckDocumentType.Items.Add(ResourcesRest.All);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        protected async void OnNewDocument(object sender, EventArgs args)
        {
            try
            {
                Dictionary<DocumentType, string> DocumentList = new Dictionary<DocumentType, string>();
                List<string> ListDocs = App.DbLayer.GetDocumentList(App.Store.Oid, out DocumentList);
                string doc = "";
                DocumentType selectedDocument = null;
                if (ListDocs.Count > 0)
                {
                    var action = await DisplayActionSheet(ResourcesRest.ChooseDocumentType, ResourcesRest.Cancel, null, ListDocs.ToArray());
                    doc = action;
                    selectedDocument = DocumentList.Where(x => x.Value == doc).FirstOrDefault().Key;
                    if (action == ResourcesRest.Cancel)
                    {
                        return;
                    }
                    if (selectedDocument != null)
                    {
                        UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                        await Task.Run(() =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushAsync(new DocumentCustomerPage(selectedDocument));
                            });
                            UserDialogs.Instance.HideLoading();
                        });
                    }
                    else
                    {
                        throw new Exception("No Valid DocumentType Found");
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void DeleteDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                var answer = await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.msgConfirmDelete, ResourcesRest.MsgYes, ResourcesRest.MsgNo);
                if (answer == true)
                {
                    Guid HeaderOid = (Guid)menuItem.CommandParameter;
                    DocumentHeader currentDocument = App.DbLayer.LoadDocumentFromDatabase(HeaderOid);
                    App.DbLayer.RemoveDocumentHeaders(currentDocument);
                    await LoadOrders(DocumentStatusOid, DocumentTypeOid, srchDescription.Text, DateFrom, DateTo, searchCriteria);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        async void SearchDocument(object sender, EventArgs e)
        {
            try
            {
                IsShowingFirstTime = false;
                string searchText = srchDescription.Text;
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                await Task.Run(async () =>
                {
                    await LoadOrders(DocumentStatusOid, DocumentTypeOid, srchDescription.Text, DateFrom, DateTo, searchCriteria);
                });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!IsShowingFirstTime)
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                Task.Run(async () =>
                {
                    await LoadOrders(DocumentStatusOid, DocumentTypeOid, srchDescription.Text, DateFrom, DateTo, searchCriteria);
                });
                UserDialogs.Instance.HideLoading();
            }
        }

        public void DescriptionFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void DescriptionUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void BarcodeFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void BarcodeUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
    }
}