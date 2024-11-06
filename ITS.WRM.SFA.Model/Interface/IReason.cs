using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IReason : ILookUp2Fields
    {
        IReasonCategory Category { get; set; }
    }
}
