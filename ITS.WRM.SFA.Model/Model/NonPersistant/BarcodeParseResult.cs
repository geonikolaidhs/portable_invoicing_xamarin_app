using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class BarcodeParseResult
    {
        public BarcodeParsingResult BarcodeParsingResult { get; set; }
        public string DecodedCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal CodeValue { get; set; }
        public string PLU { get; set; }
        public Guid? BarcodeType { get; set; }

        public BarcodeParseResult()
        {
            SetDefaultValues();
        }

        public void SetDefaultValues()
        {
            this.BarcodeParsingResult = BarcodeParsingResult.NONE;
            this.DecodedCode = null;
            this.Quantity = 0;
            this.CodeValue = 0;
            this.PLU = null;
            this.BarcodeType = null;
        }
    }
}
