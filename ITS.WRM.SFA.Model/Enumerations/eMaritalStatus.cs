using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eMaritalStatus
    {
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Other")]
        OTHER,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Married")]
        MARRIED,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MaritalStatusSingle")]
        SINGLE,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Widowed")]
        WIDOWED,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Divorced")]
        DIVORCED

    }
}
