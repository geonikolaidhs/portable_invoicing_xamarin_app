using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public class Report
    {
        public string TerminalDescription { get; set; }
        public int DiscountsQty { get; set; }
        public decimal DiscountsAmount { get; set; }
        public int DocumentDiscountsQty { get; set; }
        public decimal DocumentDiscountsAmount { get; set; }
        public int InvoicesQty { get; set; }
        public decimal InvoicesAmount { get; set; }

        public int OtherDocumentsQty { get; set; }
        public decimal OtherDocumentsAmount { get; set; }

        public int ReceiptsQty { get; set; }
        public decimal ReceiptsAmount { get; set; }
        public int CanceledQty { get; set; }
        public decimal CanceledAmount { get; set; }
        public int CanceledReturnsQty { get; set; }
        public decimal CanceledReturnsAmount { get; set; }
        public int CanceledDocumentsQty { get; set; }
        public decimal CanceledDocumentsAmount { get; set; }
        public decimal LoyaltyPointsQty { get; set; }
        public decimal LoyaltyPointsAmount { get; set; }
        public int DrawersOpenOccured { get; set; }
        public int CouponsQty { get; set; }
        public decimal CouponsAmmount { get; set; }
        public decimal ItemsQty { get; set; }
        public decimal ItemsAmount { get; set; }
        public int ItemsIncreasedQty { get; set; }
        public decimal ItemsIncreasedAmount { get; set; }
        public int DocumentsIncreasedQty { get; set; }
        public decimal DocumentsIncreasedAmount { get; set; }
        public decimal SwitchesReturnsQty { get; set; }
        public decimal SwitchesReturnsAmount { get; set; }

        public int WithdrawsQty { get; set; }
        public decimal WithdrawsAmount { get; set; }
        public decimal WithdrawsNegativeAmount { get; set; }

        public int DepositsQty { get; set; }
        public decimal DepositsAmount { get; set; }

        /// <summary>
        /// Represents the cash amount in the registry
        /// </summary>
        public decimal CashAmount { get; set; }

        /// <summary>
        /// Represents the cash amount that was inserted or removed by the cashier. 0 in Z reports
        /// </summary>
        public decimal CashierCashAmount { get; set; }
        public decimal StartingCashAmount { get; set; }

        public DateTime DateIssued { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public string VatFactor1 { get; set; }
        public decimal VatAmount1 { get; set; }
        public decimal NetAmount1 { get; set; }
        public decimal GrossAmount1 { get; set; }

        public string VatFactor2 { get; set; }
        public decimal VatAmount2 { get; set; }
        public decimal NetAmount2 { get; set; }
        public decimal GrossAmount2 { get; set; }

        public string VatFactor3 { get; set; }
        public decimal VatAmount3 { get; set; }
        public decimal NetAmount3 { get; set; }
        public decimal GrossAmount3 { get; set; }

        public string VatFactor4 { get; set; }
        public decimal VatAmount4 { get; set; }
        public decimal NetAmount4 { get; set; }
        public decimal GrossAmount4 { get; set; }

        public string VatFactor5 { get; set; }
        public decimal VatAmount5 { get; set; }
        public decimal NetAmount5 { get; set; }
        public decimal GrossAmount5 { get; set; }

        public Guid PaymentMethod1Guid { get; set; }
        public string PaymentMethod1 { get; set; }
        public decimal PaymentMethodAmount1 { get; set; }
        public decimal PaymentMethodQty1 { get; set; }

        public Guid PaymentMethod2Guid { get; set; }
        public string PaymentMethod2 { get; set; }
        public decimal PaymentMethodAmount2 { get; set; }
        public decimal PaymentMethodQty2 { get; set; }

        public Guid PaymentMethod3Guid { get; set; }
        public string PaymentMethod3 { get; set; }
        public decimal PaymentMethodAmount3 { get; set; }
        public decimal PaymentMethodQty3 { get; set; }

        public Guid PaymentMethod4Guid { get; set; }
        public string PaymentMethod4 { get; set; }
        public decimal PaymentMethodAmount4 { get; set; }
        public decimal PaymentMethodQty4 { get; set; }

        public Guid PaymentMethod5Guid { get; set; }
        public string PaymentMethod5 { get; set; }
        public decimal PaymentMethodAmount5 { get; set; }
        public decimal PaymentMethodQty5 { get; set; }

        public int ReportNumber { get; set; }

        public string UserName { get; set; }
        public List<ReportPaymentMethod> PaymentMethods { get; set; }
        public decimal UserCashFinalAmount { get; set; }
        public decimal CashAmountDifference { get; set; }


        public decimal CanceledEDPSPayments { get; set; }
        public decimal CanceledEDPSPaymentsAmount { get; set; }

        public decimal CanceledCardLinkPayments { get; set; }
        public decimal CanceledCardLinkPaymentsAmount { get; set; }
    }

    public class ReportPaymentMethod
    {
        public Guid PaymentMethodOid { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentMethodName { get; set; }
        public int Qty { get; set; }
        public decimal NegativeAmount { get; set; }
        public decimal Amount { get; set; }
        public bool IncreasesDrawerAmount { get; set; }
    }
}