using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum CouponCanBeUsedMessage
    {
        UNDEFINED,
        NOT_ACTIVE_YET,// errorMessage = string.Format(Resources.COUPON_IS_ACTIVE_FROM_UNTIL, new DateTime(IsActiveFrom).ToString(), new DateTime(IsActiveUntil).ToString()
        EXPIRED, // errorMessage = string.Format(Resources.COUPON_IS_ACTIVE_FROM_UNTIL, new DateTime(IsActiveFrom).ToString(), new DateTime(IsActiveUntil).ToString())
        ALREADY_USED,//errorMessage = string.Format(Resources.COUPON_HAS_ALREADY_BEEN_USED_TIMES, NumberOfTimesUsed);
        USABLE
    }
}
