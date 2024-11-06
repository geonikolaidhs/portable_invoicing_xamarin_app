using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface ILinkedItem: IBaseObj
    {
        IItem Item { get; set; }
        IItem SubItem { get; set; }
        double QtyFactor { get; set; }
    }
}
