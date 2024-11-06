using System;
using System.Collections.Generic;
using System.Text;
using ITS.WRM.SFA.Model.Model;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IOwner
    {
        ICompanyNew Owner { get; }
        String Description { get; }
    }

    public interface IRequiredOwner : IOwner
    {

    }
}
