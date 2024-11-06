using ITS.WRM.SFA.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class ItemPresent
    {
        public Guid Oid { get; set; }
        public Guid BarcodeOid { get; set; }
        public string Itemcode { get; set; }
        public string Name { get; set; }
        public string VatDescription { get; set; }
        public string BarcodeCode { get; set; }
        public string MeasurementDescription { get; set; }
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
                if (Qty == 0)
                    return 0;
                if (PackingMeasurementUnitRelationFactor == 0)
                    return Qty;

                return Math.Round((Qty / (decimal)PackingMeasurementUnitRelationFactor), 3, MidpointRounding.AwayFromZero);
            }
        }

        private MeasurementUnit _MeasurementUnit;
        public MeasurementUnit MeasurementUnit
        {
            get
            {
                if (_MeasurementUnit == null && MeasurementUnitOid != null && MeasurementUnitOid != Guid.Empty)
                {
                    _MeasurementUnit = DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Where(x => x.Oid == MeasurementUnitOid).FirstOrDefault();
                }
                return _MeasurementUnit;
            }
            set { _MeasurementUnit = value; }
        }

        public int IsActive { get; set; }
        public Guid Root { get; set; }
        public Guid Node { get; set; }
        public long CreatedOnTicks { get; set; }
        public long UpdatedOnTicks { get; set; }
        public decimal Qty { get; set; }
        public double Stock { get; set; }
        public decimal UnitPriceWithVat { get; set; }
        public decimal UnitPriceWithoutVat { get; set; }
        public bool SupportDecimal { get; set; }
        public bool ShowStock { get; set; }
        //the quantity exists on open documents
        public decimal ReservedQty { get; set; }

        public double PackingMeasurementUnitRelationFactor { get; set; }



        public string DisplayQty
        {
            get
            {
                return Qty.ToString();
            }
        }
        public string DisplayStock
        {
            get
            {

                return ShowStock ? CalculatedStock.ToString() : "-";

            }
        }
        public string DisplayDescription
        {
            get
            {
                return Name;
            }
        }

        public double CalculatedStock
        {
            get
            {
                return Stock - (double)(QtyOnCurrentDocument + QtyOnTemporaryList);
            }
        }

        public decimal QtyOnCurrentDocument { get; set; } = 0;

        public decimal QtyOnTemporaryList { get; set; } = 0;


        public void CalculateReservedQty(Dictionary<Guid, decimal> ItemsPreservedQuantity)
        {
            decimal val = 0;
            ItemsPreservedQuantity.TryGetValue(this.Oid, out val);
            ReservedQty = val;
        }

    }
}
