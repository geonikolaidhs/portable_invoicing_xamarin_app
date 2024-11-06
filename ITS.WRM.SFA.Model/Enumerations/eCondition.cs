using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eCondition
    {
        NONE,//
        SINGLEQUANTITY,//
        MULTIQUANTITY, //
        RECEIPT,//
        PROFORMA, //
        NONZEROLINEDISCOUNT, //
        NONZERODOCUMENTDISCOUNT,//
        HASCHANGE,//
        NONDEFAULTCUSTOMER,
        DOESNOTINCREASEDRAWERAMOUNT,
        INCREASESDRAWERAMOUNT

    }
}
