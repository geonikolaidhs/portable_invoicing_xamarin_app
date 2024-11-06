using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public class ReceiptPart
    {
        public List<ReceiptElement> Elements { get; set; }

        public ReceiptPart()
        {
            Elements = new List<ReceiptElement>();
        }
    }
}
