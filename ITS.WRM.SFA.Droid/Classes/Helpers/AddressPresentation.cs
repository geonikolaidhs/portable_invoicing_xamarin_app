using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public class AddressPresentation
    {
        public Guid Oid { get; set; }
        public string AddressDescription { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
