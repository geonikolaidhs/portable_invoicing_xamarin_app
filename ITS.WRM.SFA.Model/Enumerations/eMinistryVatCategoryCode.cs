using ITS.WRM.SFA.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eMinistryVatCategoryCode
    {
        NONE = 0,
        [WrmDescription("A")]
        A = 1,
        [WrmDescription("Β")]
        B = 2,
        [WrmDescription("Γ")]
        C = 3,
        [WrmDescription("Δ")]
        D = 4,
        [WrmDescription("Ε")]
        E = 5
    }
}
