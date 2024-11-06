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

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 1050, Permissions = eUpdateDirection.NONE)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class DocumentDetail : BaseObj
    {
        public DocumentDetail()
        {
            this.DocumentDetailDiscounts = new List<DocumentDetailDiscount>();
        }
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public Barcode Barcode { get; set; }

        [Column("Barcode")]
        [JsonProperty("Barcode.Oid")]
        [ForeignKey(typeof(Barcode))]
        [Indexed]
        public Guid BarcodeOid { get; set; }

        [JsonIgnore]
        public string BarcodeCode { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public Store CentralStore { get; set; }

        [Column("CentralStore")]
        [ForeignKey(typeof(Store))]
        [JsonProperty("CentralStore.Oid")]

        public Guid CentralStoreOid { get; set; }

        public decimal CurrentPromotionDiscountValue { get; set; }

        public string CustomDescription { get; set; }

        public string CustomMeasurementUnit { get; set; }

        public decimal CustomUnitPrice { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public DocumentHeader DocumentHeader { get; set; }

        [Indexed]
        [Column("DocumentHeader")]
        [ForeignKey(typeof(DocumentHeader))]
        [JsonProperty("DocumentHeader.Oid")]
        public Guid DocumentHeaderOid { get; set; }

        public decimal FinalUnitPrice { get; set; }

        public decimal FirstDiscount { get; set; }

        public bool FromScanner { get; set; }

        public decimal GrossTotal { get; set; }

        public decimal GrossTotalBeforeDiscount { get; set; }

        public decimal GrossTotalBeforeDocumentDiscount { get; set; }

        public decimal GrossTotalDeviation { get; set; }

        public bool HasCustomDescription { get; set; }

        public bool HasCustomPrice { get; set; }

        public bool IsCanceled { get; set; }

        public int IsOffer { get; set; }

        public bool IsPOSGeneratedPriceCatalogDetailApplied { get; set; }

        public bool IsReturn { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Item Item { get; set; }

        [Column("Item")]
        [ForeignKey(typeof(Item))]
        [JsonProperty("Item.Oid")]
        [Indexed]
        public Guid ItemOid { get; set; }
        public string ItemCode { get; set; }

        public string ItemVatCategoryDescription { get; set; }

        public int LineNumber { get; set; }

        public Guid LinkedLine { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]

        public MeasurementUnit MeasurementUnit { get; set; }

        [Column("MeasurementUnit")]
        [ForeignKey(typeof(MeasurementUnit))]
        [JsonProperty("MeasurementUnit.Oid")]
        [Indexed]
        public Guid MeasurementUnitOid { get; set; }

        public string MeasurementUnitCode { get; set; }

        public decimal NetTotal { get; set; }

        public decimal NetTotalBeforeDiscount { get; set; }

        public decimal NetTotalDeviation { get; set; }

        public string OfferDescription { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public MeasurementUnit PackingMeasurementUnit { get; set; }

        [Column("PackingMeasurementUnit")]
        [ForeignKey(typeof(MeasurementUnit))]
        [JsonProperty("PackingMeasurementUnit.Oid")]
        [Indexed]
        public Guid PackingMeasurementUnitOid { get; set; }

        public double PackingMeasurementUnitRelationFactor { get; set; }

        public decimal PackingQuantity { get; set; }

        public decimal Points { get; set; }

        public string POSGeneratedPriceCatalogDetailSerialized { get; set; }

        public decimal PriceListUnitPrice { get; set; }

        public decimal PriceListUnitPriceWithoutVAT { get; set; }

        public decimal PriceListUnitPriceWithVAT { get; set; }

        public decimal PromotionsLineDiscountsAmount { get; set; }


        public decimal Qty { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Reason Reason { get; set; }

        [ForeignKey(typeof(Reason))]
        [Column("Reason")]
        [JsonProperty("Reason.Oid")]

        public Guid ReasonOid { get; set; }
        public string Remarks { get; set; }

        public decimal SecondDiscount { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public SpecialItem SpecialItem { get; set; }

        [ForeignKey(typeof(SpecialItem))]
        [JsonProperty("SpecialItem.Oid")]
        [Column("SpecialItem")]

        public Guid SpecialItemOid { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal TotalDiscountAmountWithoutVAT { get; set; }

        public decimal TotalDiscountAmountWithVAT { get; set; }

        public decimal TotalVatAmount { get; set; }

        public decimal TotalVatAmountBeforeDiscount { get; set; }

        public decimal TotalVatAmountDeviation { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal VatFactor { get; set; }

        public string VatFactorCode { get; set; }

        public string ItemName { get; set; }

        public Guid VatFactorGuid { get; set; }

        public string MeasurementUnit2Code { get; set; }

        public string WithdrawDepositTaxCode { get; set; }


        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<DocumentDetailDiscount> DocumentDetailDiscounts { get; set; }



        public bool IsLinkedLine
        {
            get
            {
                return LinkedLine != Guid.Empty;
            }
        }

        public bool ShouldBeSummarised
        {
            get
            {
                return !this.IsCanceled;
            }
        }
        public decimal TotalDiscountIncludingVAT
        {
            get
            {
                return GrossTotalBeforeDiscount - GrossTotal;
            }
        }

        /// <summary>
        /// Calculated. All the line discounts that are not applied to the document header
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal TotalNonDocumentDiscount
        {
            get
            {
                IEnumerable<DocumentDetailDiscount> nonDocumentDiscounts = this.DocumentDetailDiscounts
                    .Where(x => x.DiscountSource != eDiscountSource.DOCUMENT &&
                                x.DiscountSource != eDiscountSource.POINTS &&
                                x.DiscountSource != eDiscountSource.CUSTOMER &&
                                x.DiscountSource != eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);

                if (nonDocumentDiscounts.Count() > 0)
                {
                    return nonDocumentDiscounts.Sum(x => x.Value);
                }
                return 0;
            }
        }

        [Ignore]
        [JsonIgnore]
        public IEnumerable<DocumentDetailDiscount> NonHeaderDocumentDetailDiscounts
        {
            get
            {
                return this.DocumentDetailDiscounts.Where(x =>
                                x.DiscountSource != eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT &&
                                x.DiscountSource != eDiscountSource.DOCUMENT &&
                                x.DiscountSource != eDiscountSource.POINTS &&
                                x.DiscountSource != eDiscountSource.CUSTOMER &&
                                x.DiscountSource != eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);
            }

        }


        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal FinalUnitPriceWithVat
        {
            get
            {
                if (Qty == 0)
                {
                    return 0;
                }
                return GrossTotalBeforeDiscount / (decimal)Qty;

            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal TotalDiscountPercentage
        {
            get
            {
                if (NetTotal + TotalDiscountAmountWithoutVAT == 0)
                {
                    return 0;
                }
                return (this.TotalDiscountAmountWithoutVAT) / (NetTotal + TotalDiscountAmountWithoutVAT);
            }
        }


        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal TotalDiscountPercentageWithVat
        {
            get
            {
                if (GrossTotal + TotalDiscountAmountWithVAT == 0)
                {
                    return 0;
                }
                return (this.TotalDiscountAmountWithVAT) / (GrossTotal + TotalDiscountAmountWithVAT);
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public string TotalDiscountPercentagePrint
        {
            get
            {
                return String.Format("{0:P2}.", TotalDiscountPercentage);
            }
        }


        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public string TotalDiscountPercentageWithVatPrint
        {
            get
            {
                return String.Format("{0:P2}.", TotalDiscountPercentageWithVat);
            }
        }



        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal CustomDiscountsAmount
        {
            get
            {
                var customDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (customDiscounts.Count() > 0)
                {
                    return customDiscounts.Sum(x => x.Value);
                }

                return 0;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal PriceCatalogDiscountPercentage
        {
            get
            {
                DocumentDetailDiscount pcDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG);
                if (pcDiscount != null)
                {
                    return pcDiscount.Percentage;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal PriceCatalogDiscountAmount
        {
            get
            {
                DocumentDetailDiscount pcDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG);
                if (pcDiscount != null)
                {
                    return pcDiscount.Value;
                }

                return 0.0m;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal NetTotalBeforeDocumentDiscount
        {
            get
            {
                DocumentDetailDiscount docDiscount = this.DocumentDetailDiscounts.FirstOrDefault(x => x.DiscountSource == eDiscountSource.DOCUMENT);
                if (docDiscount != null)
                {
                    return this.NetTotal - docDiscount.Value;
                }
                else
                {
                    return this.NetTotal;
                }
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal UnitPriceAfterDiscount
        {
            get
            {
                if (this.Qty == 0)
                {
                    return 0;
                }
                return this.NetTotal / this.Qty;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal CustomDiscountsPercentageWholeSale
        {
            get
            {
                var customDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (customDiscounts.Count() > 0)
                {
                    return customDiscounts.OrderBy(x => x.Priority).Select(x => x.Percentage).Aggregate((first, second) => (1 + first) * (1 + second) - 1);
                }
                return 0;
            }
        }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal CustomDiscountsPercentageRetail
        {
            get
            {
                var customDiscounts = this.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM);
                if (customDiscounts.Count() > 0)
                {
                    return customDiscounts.OrderBy(x => x.Priority).Select(x => x.Percentage).Aggregate((first, second) => (1 + first) * (1 + second) - 1);
                }
                return 0;
            }
        }




        [Ignore]
        [JsonIgnore]
        public string MeasurementUnitsQuantities
        {
            get
            {
                if (this.MeasurementUnit != null && this.PackingMeasurementUnit != null && this.MeasurementUnit.Oid != this.PackingMeasurementUnit.Oid)
                {
                    string quantitiesString = "";
                    quantitiesString = this.PackingQuantity.ToString();
                    if (this.PackingMeasurementUnit != null)
                    {
                        quantitiesString += " " + this.PackingMeasurementUnit.Description;
                    }
                    quantitiesString += String.Format(" ~ {0} {1}", this.Qty, this.MeasurementUnit.Description);
                    return quantitiesString;
                }
                return "";
            }
        }



    }
}
