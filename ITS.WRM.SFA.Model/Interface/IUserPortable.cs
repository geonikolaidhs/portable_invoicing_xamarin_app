using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IUserPortable: IPersistentObjectPortable
    {
        string UserName { get; }

        string FullName { get; }

        string Password { get; }

        string TaxCode { get; }

        IEnumerable<IUserTypeAccessPortable> UserTypeAccesses { get; }

        IRolePortable Role { get; }

        string Key { get; }
    }
}