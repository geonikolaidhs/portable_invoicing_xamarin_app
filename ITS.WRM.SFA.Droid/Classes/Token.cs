using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Droid.Classes
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires { get; set; }
        public string userName { get; set; }
        public string CompanyTaxCode { get; set; }
    }
}
