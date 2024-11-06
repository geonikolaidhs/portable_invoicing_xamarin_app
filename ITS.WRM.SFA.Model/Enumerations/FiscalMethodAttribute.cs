using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class FiscalMethodAttribute : Attribute
    {
        public eFiscalMethod FiscalMethod { get; protected set; }

        public FiscalMethodAttribute(eFiscalMethod fiscalMethod)
        {
            FiscalMethod = fiscalMethod;
        }

    }
}
