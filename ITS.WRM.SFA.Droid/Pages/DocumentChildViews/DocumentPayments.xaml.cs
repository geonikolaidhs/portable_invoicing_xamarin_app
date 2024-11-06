using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
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

namespace ITS.WRM.SFA.Droid.Pages.DocumentChildViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentPayments : ContentPage
    {
        DocumentHeader _DocumentHeader;
        ObservableCollection<PaymentMethod> ObservablePayments = new ObservableCollection<PaymentMethod>();
        List<DocumentPayment> PaymentList = new List<DocumentPayment>();
        ObservableCollection<DocumentPayment> ObservableDocumentPayments = new ObservableCollection<DocumentPayment>();
        PaymentMethod DocumentTypeDefaultPaymentMethod = null;
        int SelectedIndex = -1;
        bool IsShowingFirstTime = true;
        private decimal LastBalance;
        private decimal LastAmount;
        private Guid PaymentOid;
        private string msgAlert;
        private string msgBtnOK;
        private string msgNullPaymentMethod;
        private string msgNotSupportThisPayment;


        public DocumentPayments(DocumentHeader documentHeader)
        {
            InitializeComponent();
            _DocumentHeader = documentHeader;
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            InitiallizeControllers();
            LoadPaymentMethods();
            LoadDocumentPayments();

            CalculateChange();
            IsShowingFirstTime = false;

            pckPaymentMethod.SelectedIndexChanged += (sender, args) =>
            {
                if (pckPaymentMethod.SelectedIndex == -1)
                {
                    PaymentOid = Guid.Empty;
                }
                else
                {
                    string name = pckPaymentMethod.Items[pckPaymentMethod.SelectedIndex];
                    PaymentOid = ObservablePayments.ToList().Find(x => x.Description.Equals(name))?.Oid ?? Guid.Empty;
                }
            };
        }

        protected async void SavePayment(object sender, EventArgs e)
        {
            try
            {
                DocumentPayment documentPayment = null;
                List<DocumentPayment> documentPaymentList = new List<DocumentPayment>();
                PaymentMethod paymentMethod = ObservablePayments.ToList().Where(x => x.Oid == PaymentOid).FirstOrDefault();
                if (_DocumentHeader.IsSynchronized)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.DocumentAlreadySend, ResourcesRest.MsgBtnOk);
                    return;
                }
                if (_DocumentHeader.GrossTotal <= 0 && _DocumentHeader.Division != eDivision.Financial)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, "Zero Document Value", ResourcesRest.MsgBtnOk);
                    return;
                }
                if (paymentMethod == null)
                {
                    await DisplayAlert(msgAlert, msgNullPaymentMethod, msgBtnOK);
                    return;
                }
                decimal Balance;
                decimal Amount;
                txtAmount.Text.TryParse(out Amount);
                if (Amount == 0)
                {
                    return;
                }
                if (Amount < 0)
                {
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, "Δεν επιτρέπονται αρνητικές πληρωμές", ResourcesRest.MsgBtnOk);
                }
                if (_DocumentHeader.Division == eDivision.Financial)
                {
                    SetFinancialPayment(_DocumentHeader, Amount, paymentMethod);
                    return;
                }
                bool replace = false;
                decimal previousAmount = 0;
                if (_DocumentHeader.DocumentPayments != null)
                {
                    documentPayment = _DocumentHeader.DocumentPayments.Where(x => x.PaymentMethodOid == PaymentOid && !x.IsChange).FirstOrDefault();
                    documentPaymentList = _DocumentHeader.DocumentPayments;
                    if (documentPayment != null)
                    {
                        previousAmount = documentPayment.Amount;
                        replace = true;
                    }
                }
                if (documentPayment == null)
                {
                    documentPayment = new DocumentPayment();
                }
                documentPayment.Amount = Amount + previousAmount;
                documentPayment.PaymentMethod = paymentMethod;

                if (_DocumentHeader.DocumentPayments != null && _DocumentHeader.DocumentPayments.Where(x => !x.IsChange).ToList().Count > 0)
                {
                    decimal positivePayments = _DocumentHeader.DocumentPayments.Where(x => x.IsChange == false).Sum(x => x.Amount);
                    Balance = replace ? _DocumentHeader.GrossTotal - positivePayments : _DocumentHeader.GrossTotal - (positivePayments + Amount);
                }
                else
                {
                    Balance = _DocumentHeader.GrossTotal - documentPayment.Amount;
                }
                if (Balance < 0 && (documentPayment.PaymentMethod.CanExceedTotal == false || documentPayment.PaymentMethod.GiveChange == false))
                {
                    txtAmount.Text = "";
                    await DisplayAlert(msgAlert, msgNotSupportThisPayment, msgBtnOK);
                    return;
                }
                documentPayment.PaymentMethodOid = documentPayment.PaymentMethod.Oid;
                documentPayment.DocumentHeaderOid = _DocumentHeader.Oid;
                documentPayment.PaymentMethodCode = documentPayment.PaymentMethod.Code;
                documentPayment.Amount = Amount + previousAmount;
                documentPayment.DocumentHeader = _DocumentHeader;
                documentPayment.PaymentMethodDescription = documentPayment.PaymentMethod.Description;
                if (!replace)
                {
                    documentPaymentList.Add(documentPayment);
                }
                _DocumentHeader.DocumentPayments = documentPaymentList;
                App.DbLayer.InsertOrReplaceObj(documentPayment);

                CalculateChange();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        private void SetFinancialPayment(DocumentHeader document, decimal amount, PaymentMethod paymentMethod)
        {
            DocumentPayment documentPayment = null;
            bool isNew = false;
            documentPayment = document.DocumentPayments.Where(x => x.PaymentMethodOid == PaymentOid).FirstOrDefault();
            if (documentPayment == null)
            {
                documentPayment = new DocumentPayment();
                isNew = true;
            }
            documentPayment.Amount = amount;
            documentPayment.PaymentMethod = paymentMethod;
            documentPayment.PaymentMethodOid = paymentMethod.Oid;
            documentPayment.DocumentHeaderOid = document.Oid;
            documentPayment.DocumentHeader = document;
            documentPayment.PaymentMethodDescription = paymentMethod.Description;
            documentPayment.PaymentMethodCode = paymentMethod.Code;
            App.DbLayer.InsertOrReplaceObj<DocumentPayment>(documentPayment);
            if (isNew)
            {
                document.DocumentPayments.Add(documentPayment);
            }
            DocumentHelper.SetFinancialDocumentDetail(ref document);
            CalculateChange();
        }


        private void CalculateChange()
        {
            try
            {
                decimal Balance = 0;
                List<DocumentPayment> documentPaymentList = new List<DocumentPayment>();
                if (_DocumentHeader.DocumentPayments != null)
                {
                    decimal positivePayments = _DocumentHeader.DocumentPayments.Where(x => x.IsChange == false).Sum(x => x.Amount);
                    Balance = _DocumentHeader.GrossTotal - positivePayments;
                    documentPaymentList = _DocumentHeader.DocumentPayments;
                }
                DocumentPayment changePayment = App.DbLayer.GetDocumentPayments(_DocumentHeader.Oid).Where(x => x.IsChange).FirstOrDefault();
                if (changePayment != null)
                {
                    App.DbLayer.DeleteObj<DocumentPayment>(changePayment);
                    if (documentPaymentList.Where(x => x.IsChange).FirstOrDefault() != null)
                    {
                        documentPaymentList.Remove(changePayment);
                    }
                }
                if (Balance < 0)
                {
                    changePayment = new DocumentPayment();
                    PaymentMethod payMethod = ObservablePayments.ToList().Where(x => x.Oid.Equals(PaymentOid)).FirstOrDefault();
                    if (payMethod == null)
                    {
                        payMethod = ObservablePayments.ToList().Where(x => x.GiveChange && x.CanExceedTotal).FirstOrDefault();
                    }
                    changePayment.PaymentMethod = payMethod;
                    changePayment.Amount = Balance;
                    changePayment.PaymentMethodOid = changePayment.PaymentMethod.Oid;// documentPayment.PaymentMethod.Oid;
                    changePayment.PaymentMethodCode = changePayment.PaymentMethod.Code;
                    changePayment.DocumentHeaderOid = _DocumentHeader.Oid;
                    changePayment.DocumentHeader = _DocumentHeader;
                    changePayment.IsChange = true;
                    documentPaymentList.Add(changePayment);
                    App.DbLayer.InsertOrReplaceObj(changePayment);
                    _DocumentHeader.DocumentPayments = documentPaymentList;
                }

                _DocumentHeader.DocumentPayments = documentPaymentList;
                LoadDocumentPayments();
                CalculateBalance(_DocumentHeader);
                txtAmount.Text = "";
                pckPaymentMethod.SelectedIndex = SelectedIndex;
                PaymentOid = DocumentTypeDefaultPaymentMethod?.Oid ?? Guid.Empty;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }
        private void InitiallizeControllers()
        {
            lblPaymentMethodsTitle.Text = ResourcesRest.PaymentMethods;
            lblBalance.Text = ResourcesRest.Balance;
            lblPaymentType.Text = ResourcesRest.PaymentMethods;
            lblPaymentAmount.Text = ResourcesRest.Amount;
            lblTotalAmount.Text = ResourcesRest.DocumentTotal;
            btnRemainingAmount.Text = ResourcesRest.RemainingAmount;
            btnSave.Text = ResourcesRest.Save;
            pckPaymentMethod.Title = ResourcesRest.SelectPaymentMethod;
            lblPaymentAmountDescr.Text = ResourcesRest.Amount;
            lblPaymentName.Text = ResourcesRest.paymentMethod;
            msgAlert = ResourcesRest.MsgTypeAlert;
            msgBtnOK = ResourcesRest.MsgBtnOk;
            msgNullPaymentMethod = ResourcesRest.msgNoPaymentMethodFound;
            msgNotSupportThisPayment = ResourcesRest.msgNotSupportThisPayment;
        }
        private void CalculateBalance(DocumentHeader documentHeader)
        {
            decimal Balance;
            lblTotalAmountValue.Text = documentHeader.GrossTotal.ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
            if (documentHeader.DocumentPayments != null)
            {
                decimal positivePayments = _DocumentHeader.DocumentPayments.Where(x => x.IsChange == false).Sum(x => x.Amount);
                Balance = _DocumentHeader.GrossTotal - positivePayments;
            }
            else
            {
                Balance = documentHeader.GrossTotal;
            }
            Balance = Balance < 0 ? Math.Abs(Balance) : 0;
            lblBalanceValue.Text = Math.Abs(Balance).ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
        }
        protected override void OnAppearing()
        {

            if (!IsShowingFirstTime)
            {
                LoadDocumentPayments();
                decimal balance = 0;
                string strAmount = !string.IsNullOrEmpty(lblBalanceValue.Text) ? lblBalanceValue.Text.Remove(lblBalanceValue.Text.Length - 1) : "";
                strAmount.TryParse(out balance);
                decimal amount = 0;
                txtAmount.Text.TryParse(out amount);
                LastBalance = balance;
                LastAmount = amount;
                CalculateChange();
            }
            base.OnAppearing();
        }
        protected async void RecaltulateAmount(object sender, EventArgs e)
        {
            try
            {
                decimal alreadyPaid = _DocumentHeader.DocumentPayments == null ? 0 : _DocumentHeader.DocumentPayments.Sum(x => x.Amount);
                decimal total = _DocumentHeader.GrossTotal - alreadyPaid;
                txtAmount.Text = total < 0 ? txtAmount.Text = "0" : total.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        private void LoadPaymentMethods()
        {
            try
            {
                List<PaymentMethod> paymentMethods = App.DbLayer.GetPaymentMethods();
                ObservablePayments.Clear();
                pckPaymentMethod.Items.Clear();
                if (_DocumentHeader.DocumentType == null)
                {
                    _DocumentHeader.DocumentType = App.DbLayer.GetDocumentTypeById(_DocumentHeader.DocumentTypeOid);
                }
                DocumentTypeDefaultPaymentMethod = _DocumentHeader.DocumentType.GetDefaultPaymentMethod(App.DbLayer);
                int index = 0;
                foreach (PaymentMethod payment in paymentMethods)
                {
                    ObservablePayments.Add(payment);
                    pckPaymentMethod.Items.Add(payment.Description);
                    if (DocumentTypeDefaultPaymentMethod != null && payment.Oid == DocumentTypeDefaultPaymentMethod.Oid)
                    {
                        SelectedIndex = index;
                    }
                    index++;
                }
                pckPaymentMethod.SelectedIndex = SelectedIndex;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }
        private async void LoadDocumentPayments()
        {
            try
            {
                ObservableDocumentPayments.Clear();
                if (_DocumentHeader != null)
                {
                    List<DocumentPayment> ListDocumentPayments = App.DbLayer.GetDocumentPayments(_DocumentHeader.Oid);
                    foreach (DocumentPayment payment in ListDocumentPayments)
                    {
                        ObservableDocumentPayments.Add(payment);
                    }
                    _DocumentHeader.DocumentPayments = ListDocumentPayments;
                }
                DocumentPaymentListView.ItemsSource = ObservableDocumentPayments;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        async void RemovePayment(object sender, EventArgs e)
        {
            try
            {
                MenuItem menuItem = (MenuItem)sender;
                if (menuItem == null)
                {
                    return;
                }
                DocumentPayment paymentObj = ObservableDocumentPayments.Where(x => x.Oid.Equals(menuItem.CommandParameter) && !x.IsChange).FirstOrDefault();
                if (paymentObj != null)
                {
                    App.DbLayer.DeleteObj<DocumentPayment>(paymentObj);
                    _DocumentHeader.DocumentPayments.Remove(paymentObj);
                    if (_DocumentHeader.Division == eDivision.Financial)
                    {
                        DocumentHelper.SetFinancialDocumentDetail(ref _DocumentHeader);
                    }
                    CalculateChange();
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        public void AmountFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void AmountUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }
    }
}