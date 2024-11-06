using ITS.WRM.SFA.Model.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Droid.Classes
{
    public class PriceCatalogPresentation
    {
        public string PriceCatalogDescription { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime UpdatedOnDate { get; set; }
        public string Value { get; set; }
        public bool VatIncluded { get; set; }
        public bool IsActive { get; set; }
        public string Createheader { get; set; }
        public string UpdateHeader { get; set; }
        public string IsActiveHeader { get; set; }
        public string VatIncludedHeader { get; set; }
        public string VatChecked { get; set; }
        public string ActiveChecked { get; set; }
    }
}
