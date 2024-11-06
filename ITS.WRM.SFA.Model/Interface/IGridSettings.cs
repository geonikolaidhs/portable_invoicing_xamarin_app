using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IGridSettings: IBaseObj
    {
        string GridName { get; set; }
        string GridLayout { get; set; }
        //IUser User { get; set; }
    }
}
