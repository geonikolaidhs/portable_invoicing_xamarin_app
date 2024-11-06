using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public class Receipt
    {
        public List<string> Header { get; set; }
        public List<string> Body { get; set; }
        public List<string> Footer { get; set; }


        public List<string> GetReceiptLines()
        {
            List<string> allTheLines = new List<string>();
            allTheLines.AddRange(Header);
            allTheLines.AddRange(Body);
            allTheLines.AddRange(Footer);

            return allTheLines;

        }
    }
}
