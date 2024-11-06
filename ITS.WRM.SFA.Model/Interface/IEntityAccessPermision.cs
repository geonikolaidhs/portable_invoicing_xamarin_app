using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IEntityAccessPermision:IPermission
    {
        string EntityType { get; set; }
        List<IRoleEntityAccessPermision> RoleEntityAccessPermisions { get; set; }
    }
}
