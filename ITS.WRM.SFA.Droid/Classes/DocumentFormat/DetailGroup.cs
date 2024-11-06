using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public class DetailGroup : ReceiptElement
    {
        public List<ReceiptElement> Lines { get; set; }
        public DetailGroup()
        {
            Lines = new List<ReceiptElement>();
        }
    }
}
