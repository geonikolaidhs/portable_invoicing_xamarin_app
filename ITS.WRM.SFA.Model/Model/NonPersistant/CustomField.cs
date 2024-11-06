using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class CustomField:BaseObj
    {

        public string FieldName { get; set; }
        public string Label { get; set; }
        [Ignore]
        public CustomEnumerationDefinition CustomEnumeration { get; set; }
        
    }
}
