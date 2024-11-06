using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentDetailDiscount
    {
        IDocumentDetail DocumentDetail { get; set; }
        eDiscountSource DiscountSource { get; set; }
        int Priority { get; set; }
        eDiscountType DiscountType { get; set; }
        bool DiscardsOtherDiscounts { get; set; }
        decimal Value { get; set; }
        decimal Percentage { get; set; }
        IDiscountType Type { get; set; }
        string TypeDescription { get; set; }
        string Description { get; set; }
        Guid Promotion { get; set; }
        ITransactionCoupon TransactionCoupon { get; set; }
        decimal DiscountWithVAT { get; set; }
        decimal DiscountWithoutVAT { get; set; }
    }
}
