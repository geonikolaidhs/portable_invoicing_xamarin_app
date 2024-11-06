using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eStoreDocumentType
    {
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "NONE")]
        [EnumMember]
        NONE,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Order")]
        [EnumMember]
        ORDER
    }
}
