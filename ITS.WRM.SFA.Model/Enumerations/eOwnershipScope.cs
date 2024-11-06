using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eOwnershipScope
    {
        [WrmDescription("Global")]
        Global,
        [WrmDescription("Owner")]
        Owner
    }
}
