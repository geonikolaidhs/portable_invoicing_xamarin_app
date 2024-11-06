using ITS.WRM.SFA.Model.Model;
using System;
using Xamarin.Forms;
using System.Linq;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class ItemDetail
    {
        public Item Item { get; set; }

        public Barcode Barcode { get; set; }

        public ItemBarcode ItemBarcode { get; set; }

        public Guid ItemOid { get; set; }

        public Guid BarcodeOid { get; set; }

        public Guid MeasurementUnitOid { get; set; }

        public Guid PackingMeasurementUnitOid { get; set; }

        private MeasurementUnit _PackingMeasurementUnit;

        public MeasurementUnit PackingMeasurementUnit
        {
            get
            {
                if (_PackingMeasurementUnit == null && PackingMeasurementUnitOid != null && PackingMeasurementUnitOid != Guid.Empty)
                {
                    _PackingMeasurementUnit = DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Where(x => x.Oid == PackingMeasurementUnitOid).FirstOrDefault();
                }
                return _PackingMeasurementUnit;
            }
            set { _PackingMeasurementUnit = value; }
        }

        public decimal PackingQuantity
        {
            get
            {
                if (TotalQty == 0)
                    return 0;
                if (PackingMeasurementUnitRelationFactor == 0)
                    return TotalQty;

                return Math.Round((TotalQty / (decimal)PackingMeasurementUnitRelationFactor), 3, MidpointRounding.AwayFromZero);
            }
        }

        public double PackingMeasurementUnitRelationFactor { get; set; }

        public MeasurementUnit MeasurementUnit { get; set; }

        public string Barcodecode { get; set; }

        public string Itemcode { get; set; }

        public decimal TotalQty { get; set; }

        public decimal UnitPriceWithVat { get; set; }

        public decimal UnitPriceWithoutVat { get; set; }

        public decimal TotalValueWithVat { get; set; }

        public decimal TotalValueWithoutVat { get; set; }

        public double Stock { get; set; }

        public long UpdatedOnTicks { get; set; }

        public bool ShowStock { get; set; }

        public ItemPresent ConvertToItemPresent()
        {
            MeasurementUnit mu = DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Find(x => x.Oid == MeasurementUnitOid);
            VatCategory vc = DependencyService.Get<ICrossPlatformMethods>().GetVatCategories().Find(x => x.Oid == Item.VatCategoryOid);
            ItemPresent present = new ItemPresent();
            present.Oid = ItemOid;
            present.BarcodeCode = Barcodecode;
            present.Itemcode = Itemcode;
            present.BarcodeOid = BarcodeOid;
            present.MeasurementUnitOid = MeasurementUnitOid;
            present.MeasurementUnit = mu;
            present.MeasurementDescription = mu?.Description ?? "";
            present.Name = Item.Name;
            present.Stock = Item.Stock;
            present.VatDescription = vc?.Description ?? "";
            present.SupportDecimal = mu?.SupportDecimal ?? false;
            present.UnitPriceWithoutVat = UnitPriceWithoutVat;
            present.UnitPriceWithVat = UnitPriceWithVat;
            present.Qty = TotalQty;
            present.UpdatedOnTicks = UpdatedOnTicks;
            present.ShowStock = ShowStock;
            present.PackingMeasurementUnitOid = PackingMeasurementUnitOid;
            present.PackingMeasurementUnit = PackingMeasurementUnit;
            present.PackingMeasurementUnitRelationFactor = PackingMeasurementUnitRelationFactor;
            return present;
        }


    }
}
