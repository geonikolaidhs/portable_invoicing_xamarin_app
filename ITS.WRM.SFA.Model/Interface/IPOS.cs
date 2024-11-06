using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPOS:ITerminal
    {
        bool PrintDiscountAnalysis { get; set; }
        IDocumentSeries ProFormaInvoiceDocumentSeries { get; set; }
        bool UsesTouchScreen { get; set; }
        IPOSPrintFormat ZFormat { get; set; }
        IPOSPrintFormat XFormat { get; set; }
        IPOSPrintFormat ReceiptFormat { get; set; }
        eCultureInfo CultureInfo { get; set; }
        string ABCDirectory { get; set; }
        char ReceiptVariableIdentifier { get; set; }
        eFiscalDevice FiscalDevice { get; set; }
        eFiscalMethod FiscalMethod { get; set; }
        ICustomer DefaultCustomer { get; set; }
        IDocumentType DefaultDocumentType { get; set; }
        IDocumentType ProFormaInvoiceDocumentType { get; set; }
        IDocumentType WithdrawalDocumentType { get; set; }
        IDocumentType DepositDocumentType { get; set; }
        IDocumentSeries WithdrawalDocumentSeries { get; set; }
        IDocumentSeries DepositDocumentSeries { get; set; }
        ISpecialItem WithdrawalItem { get; set; }
        ISpecialItem DepositItem { get; set; }
        IDocumentSeries DefaultDocumentSeries { get; set; }
        IDocumentStatus DefaultDocumentStatus { get; set; }
        IPaymentMethod DefaultPaymentMethod { get; set; }
        //IPOSKeysLayout POSKeysLayout { get; set; }
        //IPOSLayout POSLayout { get; set; }
        bool UsesKeyLock { get; set; }
        //IPOSActionLevelsSet POSActionLevelsSet { get; set; }
        char CurrencySymbol { get; set; }
        eCurrencyPattern CurrencyPattern { get; set; }
        bool AutoFocus { get; set; }
        bool AutoIssueZEAFDSS { get; set; }
        bool AsksForStartingAmount { get; set; }
        bool EnableLowEndMode { get; set; }
        bool DemoMode { get; set; }
        bool AsksForFinalAmount { get; set; }
        eForcedWithdrawMode ForcedWithdrawMode { get; set; }
        decimal ForcedWithdrawCashAmountLimit { get; set; }
        string StandaloneFiscalOnErrorMessage { get; set; }
        IDocumentType SpecialProformaDocumentType { get; set; }
        IDocumentSeries SpecialProformaDocumentSeries { get; set; }
    }
}
