
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public abstract class ReceiptElement
    {
        public eSource Source { get; set; }
        public eCondition Condition { get; set; }

    }
}
