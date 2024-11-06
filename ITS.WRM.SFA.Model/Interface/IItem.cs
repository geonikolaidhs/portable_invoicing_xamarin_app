using ITS.WRM.SFA.Model.Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IItem : IBasicObj, IRequiredOwner
    {
        IMeasurementUnit PackingMeasurementUnit { get; set; }
        //string ImageDescription { get; set; }
        //string ImageInfo { get; set; }
        //IItemCustomPriceOptions CustomPriceOptions { get; set; }
        bool AcceptsCustomDescription { get; set; }
        decimal ReferenceUnit { get; set; }
        string Remarks { get; set; }
        decimal Points { get; set; }
        double MinOrderQty { get; set; }
        double PackingQty { get; set; }
        double OrderQty { get; set; }
        double MaxOrderQty { get; set; }
        string Code { get; set; }

        IBarcode DefaultBarcode { get; set; }
        IVatCategory VatCategory { get; set; }
        Guid? MotherCodeOid { get; set; }
        DateTime InsertedDate { get; set; }
        ISeasonality Seasonality { get; set; }
        IBuyer Buyer { get; set; }
        //bool IsCentralStored { get; set; }
        ISupplierNew DefaultSupplier { get; set; }
        string Name { get; set; }
        bool IsTax { get; set; }
        bool DoesNotAllowDiscounts { get; set; }
        decimal ContentUnit { get; set; }
        //byte[] ExtraFile { get; set; }
        //string ExtraFilename { get; set; }
        //string ExtraMimeType { get; set; }
        //string ExtraHtml { get; set; }
        bool IsGeneralItem { get; set; }
        IItemExtraInfo ItemExtraInfo { get; set; }
        //byte[] ImageSmall { get; set; }
    }
}
