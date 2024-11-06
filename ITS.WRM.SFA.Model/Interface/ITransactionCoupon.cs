using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface ITransactionCoupon: IBaseObj
    {
        string CouponCode { get; set; }
        ICouponMask CouponMask { get; set; }
        ICoupon Coupon { get; set; }
        IDocumentHeader DocumentHeader { get; set; }
        IDocumentPayment DocumentPayment { get; set; }
        IDocumentDetailDiscount DocumentDetailDiscount { get; set; }
        bool IsCanceled { get; set; }
    }
}
