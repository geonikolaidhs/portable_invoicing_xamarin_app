using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace ITS.WRM.SFA.Model
{
    public class SFASettings
    {
        public string AuthenticationURL { get; set; }
        public string ServerURL { get; set; }
        public string DatabaseDownloadURL { get; set; }
        public int SfaId { get; set; }
        public Guid DefaultDocumentStatusOid { get; set; }
        public Guid DocumentStatusToSendOid { get; set; }
        public Guid DefaultDocumentTypeOid { get; set; }
        public Guid DocumentSeries { get; set; }
        public Guid CategoryNode { get; set; }
        public Guid DefaultStore { get; set; }
        public string Language { get; set; }
        public String BlueToothScanner { get; set; }
        public String BlueToothPrinter { get; set; }
        public Guid LoadingDocumentTypeOid { get; set; }
        public Guid UnLoadingDocumentTypeOid { get; set; }
        public int ApiTimeout { get; set; } = 30;
        public Guid UnLoadingSeriesOid { get; set; }
        public bool Zpl { get; set; }
        public string VehicleNumber { get; set; }
        public int PrinterLineChars { get; set; }
        public bool PrinterConvertEncoding { get; set; }
        [XmlIgnoreAttribute]
        public Encoding EncodingFrom { get; set; }
        [XmlIgnoreAttribute]
        public Encoding EncodingTo { get; set; }
        public string EncodingFromStr { get; set; }
        public string EncodingToStr { get; set; }



    }

}
