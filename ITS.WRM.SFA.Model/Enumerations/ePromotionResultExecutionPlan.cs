using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.WRM.SFA.Resources;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum ePromotionResultExecutionPlan
    {
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "AFTER_DOCUMENT_CLOSED")]
        AFTER_DOCUMENT_CLOSED = 0,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "BEFORE_DOCUMENT_CLOSED")]
        BEFORE_DOCUMENT_CLOSED = 1
    }
}
