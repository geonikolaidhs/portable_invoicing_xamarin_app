using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPromotion: ILookUp2Fields
    {
        string PrintedDescription { get; set; }
        Guid PromotionApplicationRuleGroupOid { get; set; }
        DaysOfWeek ActiveDaysOfWeek { get; set; }
        DateTime EndTime { get; set; }
        DateTime StartTime { get; set; }
        DateTime EndDate { get; set; }
        DateTime StartDate { get; set; }
        int MaxExecutionsPerReceipt { get; set; }
        bool TestMode { get; set; }
    }
}
