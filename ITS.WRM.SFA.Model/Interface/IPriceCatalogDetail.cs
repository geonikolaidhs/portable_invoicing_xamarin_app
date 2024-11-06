using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPriceCatalogDetail: IBaseObj
    {
        decimal OldTimeValue { get; set; }
        decimal TimeValue { get; set; }
        long TimeValueValidUntil { get; set; }
        long TimeValueValidFrom { get; set; }
        long LabelPrintedOn { get; set; }
        decimal MarkUp { get; set; }
        long ValueChangedOn { get; set; }
        decimal OldValue { get; set; }
        IPriceCatalog PriceCatalog { get; set; }
        decimal DatabaseValue { get; set; }
        bool VATIncluded { get; set; }
        decimal UnitValue { get; set; }
        long OldTimeValueValidFrom { get; set; }
        long OldTimeValueValidUntil { get; set; }
        bool LabelPrinted { get; set; }
        long TimeValueChangedOn { get; set; }
        IItem Item { get; set; }
        IBarcode Barcode { get; set; }
        decimal Discount { get; set; }
    }
}
