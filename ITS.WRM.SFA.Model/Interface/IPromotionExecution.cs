using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPromotionExecution: IBaseObj
    {
        IDiscountType DiscountType { get; set; }
        decimal Percentage { get; set; }
        decimal Value { get; set; }
        IPromotion Promotion { get; set; }
    }
}
