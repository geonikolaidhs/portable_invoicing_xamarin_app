using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IRole
    {
        ICompanyNew Owner { get; set; }
        eRoleType Type { get; set; }
    }
}
