using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 660, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogDetail : BaseObj
    {

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public Barcode Barcode { get; set; }

        [ForeignKey(typeof(Barcode))]
        [Column("Barcode")]
        [JsonProperty(PropertyName = "Barcode.Oid")]
        public Guid BarcodegOid { get; set; }

        [Column("Value")]
        public decimal DatabaseValue { get; set; }

        public decimal Discount { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public Item Item { get; set; }

        [Indexed]
        [ForeignKey(typeof(Item))]
        [Column("Item")]
        [JsonProperty(PropertyName = "Item.Oid")]
        public Guid ItemOid { get; set; }


        public bool LabelPrinted { get; set; }

        public long LabelPrintedOn { get; set; }

        public decimal MarkUp { get; set; }

        public decimal OldTimeValue { get; set; }

        public long OldTimeValueValidFrom { get; set; }

        public long OldTimeValueValidUntil { get; set; }

        public decimal OldValue { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public PriceCatalog PriceCatalog { get; set; }


        [Ignore]
        [JsonIgnore]
        public string PriceCatalogDescription { get; set; }

        [ForeignKey(typeof(PriceCatalog))]
        [Column("PriceCatalog")]
        [JsonProperty(PropertyName = "PriceCatalog.Oid")]
        public Guid PriceCatalogOid { get; set; }

        public long TimeValueValidUntil { get; set; }

        public decimal UnitValue { get; set; }

        public long ValueChangedOn { get; set; }

        public bool VATIncluded { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        public List<PriceCatalogDetailTimeValue> TimeValues { get; set; }

        public decimal Value
        {
            get
            {
                if (TimeValues != null && TimeValues.Count > 0)
                {
                    long now = DateTime.Now.Ticks;
                    PriceCatalogDetailTimeValue effectiveTimeValueObject = TimeValues.Where(x =>
                                                                                    x.TimeValueValidFrom <= now
                                                                                 && x.TimeValueValidUntil >= now
                                                                                 && x.TimeValue > 0)
                                                                                 .OrderBy(z => z.TimeValueValidUntil)
                                                                                 .FirstOrDefault();
                    if (effectiveTimeValueObject != null)
                    {
                        return effectiveTimeValueObject.TimeValue;
                    }
                }
                return DatabaseValue;
            }
        }

        public decimal GetUnitPriceWithVat()
        {
            decimal _UnitPrice;
            if (VATIncluded)
            {
                _UnitPrice = Value;
            }
            else
            {
                if (Item == null)
                {
                    Item = DependencyService.Get<ICrossPlatformMethods>().GetItem(ItemOid);
                }
                if (Item == null)
                {
                    _UnitPrice = Value;
                }
                else
                {
                    if (Item.VatCategoryOid == null || Item.VatCategoryOid == Guid.Empty)
                    {
                        _UnitPrice = Value;
                    }
                    else
                    {
                        VatLevel vatLevel = DependencyService.Get<ICrossPlatformMethods>().GetStoreVatLevel();
                        if (vatLevel == null)
                        {
                            _UnitPrice = Value;
                        }
                        else
                        {
                            VatFactor vatFactor = DependencyService.Get<ICrossPlatformMethods>().GetVatFactors().Where(x => x.VatCategoryOid == Item.VatCategoryOid && x.VatLevelOid == vatLevel.Oid).FirstOrDefault();
                            if ((vatFactor == null) || (vatFactor.Factor == 0))
                            {
                                _UnitPrice = Value;
                            }
                            else
                            {
                                _UnitPrice = Value * (1 + Math.Abs(vatFactor.Factor));
                            }
                        }
                    }
                }
            }
            return _UnitPrice;
        }

        public decimal GetUnitPriceWithoutVat()
        {
            decimal _UnitPrice;
            if (!VATIncluded)
            {
                _UnitPrice = Value;
            }
            else
            {
                if (Item == null)
                {
                    Item = DependencyService.Get<ICrossPlatformMethods>().GetItem(ItemOid);
                }
                if (Item == null)
                {
                    _UnitPrice = Value;
                }
                else
                {
                    if (Item.VatCategoryOid == null || Item.VatCategoryOid == Guid.Empty)
                    {
                        _UnitPrice = Value;
                    }
                    else
                    {
                        VatLevel vatLevel = DependencyService.Get<ICrossPlatformMethods>().GetStoreVatLevel();
                        if (vatLevel == null)
                        {
                            _UnitPrice = Value;
                        }
                        else
                        {
                            VatFactor vatFactor = DependencyService.Get<ICrossPlatformMethods>().GetVatFactors().Where(x => x.VatCategoryOid == Item.VatCategoryOid && x.VatLevelOid == vatLevel.Oid).FirstOrDefault();
                            if ((vatFactor == null) || (vatFactor.Factor == 0))
                            {
                                _UnitPrice = Value;
                            }
                            else
                            {
                                _UnitPrice = Value / (1 + Math.Abs(vatFactor.Factor));
                            }
                        }
                    }
                }
            }
            return _UnitPrice;
        }
    }
}
