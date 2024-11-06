using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPersistentObjectModel
    {
        Guid Oid { get; set; }

        DateTime CreatedOn { get; set;}

        DateTime UpdatedOn { get; set;}
    }
}
