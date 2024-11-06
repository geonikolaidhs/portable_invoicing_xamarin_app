using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class CustomerPresent
    {
        public Guid Oid { get; set; }
        public string CompanyName { get; set; }
        public string Code { get; set; }
        public string Profession { get; set; }
        public string TaxCode { get; set; }
        public string Number { get; set; }
        public string Address { get; set; }
    }
}
