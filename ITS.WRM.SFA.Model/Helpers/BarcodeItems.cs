using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class BarcodeItems
    {
        public Guid Oid { get; set; }
        public string Code { get; set; }
        public string MesurementUnit { get; set; }
        public string UpdateOn { get; set; }
        public string CreateOn { get; set; }
    }
}
