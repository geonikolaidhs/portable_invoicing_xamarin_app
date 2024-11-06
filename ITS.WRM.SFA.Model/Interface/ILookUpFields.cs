using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface ILookUpFields : IBaseObj
    {
        string Description { get; set; }
        bool Update { get; set; }
        bool IsDefault { get; set; }

    }
}
