using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IBasicObj
    {
        //Guid MasterObjOid { get; set; }
        //string MLValues { get; set; }
        Guid Oid { get; set; }
        //MemberInfoCollection _ChangedMembers;
        // static Dictionary<Type, ReflectionModelDescription> _cachedReflectionModelDescription = new Dictionary<Type, ReflectionModelDescription>();
        //static object lockobject = new object();
        //string ObjectSignature { get; set; }
        bool IsActive { get; set; }
        long UpdatedOnTicks { get; set; }
        //private User _CreatedBy { get; set; }
        string CreatedByDevice { get; set; }
        string UpdateByDevice { get; set; }
        //private User UpdatedBy { get; set; }
        long CreatedOnTicks { get; set; }
        string ReferenceId { get; set; }
    }

}
