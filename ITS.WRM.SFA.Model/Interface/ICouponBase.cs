using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface ICouponBase : IBasicObj, IRequiredOwner
    {
        long _IsActiveUntil { get; set; }
        long _IsActiveFrom { get; set; }
        bool _IsUnique { get; set; }
        ICompanyNew _Owner { get; set; }
        string _Description { get; set; }
        CouponAppliesOn _CouponAppliesOn { get; set; }
        //ICouponAmountType _CouponAmountType { get; set; }
        CouponAmountIsAppliedAs _CouponAmountIsAppliedAs { get; set; }
        // DiscountType _DiscountType { get; set; }
        //PaymentMethod _PaymentMethod { get; set; }
    }
}
