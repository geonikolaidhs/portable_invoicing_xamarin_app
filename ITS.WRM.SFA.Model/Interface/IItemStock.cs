using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IItemStock: IBaseObj
    {
        IStore Store { get; set; }
        IItem Item { get; set; }
        IBarcode Barcode { get; set; }
        double Value { get; set; }
    }
}
