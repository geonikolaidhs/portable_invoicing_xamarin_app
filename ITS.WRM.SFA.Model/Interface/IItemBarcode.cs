using ITS.WRM.SFA.Model.Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IItemBarcode: IBaseObj, IRequiredOwner
    {
        string PluPrefix { get; set; }
        string PluCode { get; set; }
        IMeasurementUnit MeasurementUnit { get; set; }
        double RelationFactor { get; set; }
        IBarcodeType Type { get; set; }
        IBarcode Barcode { get; set; }
        IItem Item { get; set; }
    }
}
