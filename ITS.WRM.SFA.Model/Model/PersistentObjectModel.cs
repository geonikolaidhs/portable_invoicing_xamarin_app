using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    public class PersistentObjectModel 
    {
        public DateTime CreatedOn { get; set; }
        
        public Guid Oid { get; set; }

        public DateTime UpdatedOn { get; set; }

    }
}
