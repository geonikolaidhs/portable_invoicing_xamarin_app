using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IActionType : IBasicObj, IRequiredOwner
    {

        //ActionEntityCategory Category { get; set; }
        eTotalizersUpdateMode UpdateMode { get; set; }
        IStore Store { get; set; }
    }
}
