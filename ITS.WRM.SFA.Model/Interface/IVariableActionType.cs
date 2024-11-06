using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IVariableActionType: IBaseObj
    {
        //Variable Variable { get; set; }
        IActionType ActionType { get; set; }
        VariableMethods VariableAction { get; set; }
        //VariableReplaceMethod VariableReplaceMethod { get; set; }
        //bool UpdateFieldOnRecalculate { get; set; }
        string VariableName { get; set; }
    }
}
