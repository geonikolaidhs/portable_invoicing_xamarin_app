using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public abstract class ReceiptLine : ReceiptElement
    {
        //public int Order { get; set; }
        public eAlignment LineAlignment { get; set; }
        public bool IsBold { get; set; }
        public uint MaxCharacters { get; set; }
    }
}
