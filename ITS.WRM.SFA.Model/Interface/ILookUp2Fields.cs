using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface ILookUp2Fields : ILookUpFields, IOwner
    {
        string Code { get; set; }
        string ReferenceCode { get; set; }
    }
}
