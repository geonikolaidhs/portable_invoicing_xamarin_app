using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Enumerations;
using SQLite;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using System.Linq;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 940, Permissions = eUpdateDirection.SFA_TO_MASTER)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class DocumentHeader : BaseObj
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AddressProfession { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Address BillingAddress { get; set; }

        [ForeignKey(typeof(Address))]
        [JsonProperty("BillingAddress.Oid")]
        [Column("BillingAddress")]
        public Guid BillingAddressOid { get; set; }

        public Guid? CanceledByDocumentOid { get; set; }

        public Guid? CancelsDocumentOid { get; set; }

        public bool CheckFromStore { get; set; }

        public decimal ConsumedPointsForDiscount { get; set; }

        public bool CouponsHaveBeenUpdatedOnMaster { get; set; }

        public bool CouponsHaveBeenUpdatedOnStoreController { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Customer Customer { get; set; }

        [ForeignKey(typeof(Customer))]
        [JsonProperty("Customer.Oid")]
        [Column("Customer")]
        public Guid CustomerOid { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerLookupCode { get; set; }

        public string CustomerLookUpTaxCode { get; set; }

        public string CustomerName { get; set; }

        public bool CustomerNotFound { get; set; }

        public string DeliveryAddress { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Trader DeliveryTo { get; set; }

        [ForeignKey(typeof(Trader))]
        [JsonProperty("DeliveryTo.Oid")]
        [Column("DeliveryTo")]
        public Guid DeliveryToOid { get; set; }

        public string DeliveryToTraderTaxCode { get; set; }


        public Guid? DenormalisedAddress { get; set; }

        public string DenormalizedCustomer { get; set; }

        public eDivision Division { get; set; }

        public decimal DocumentDiscountAmount { get; set; }

        public decimal DocumentDiscountPercentage { get; set; }

        public decimal DocumentDiscountPercentagePerLine { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public DiscountType DocumentDiscountType { get; set; }

        [ForeignKey(typeof(DiscountType))]
        [JsonProperty("DocumentDiscountType.Oid")]
        [Column("DocumentDiscountType")]
        public Guid DocumentDiscountTypeOid { get; set; }

        public bool DocumentFinished { get; set; }

        public int DocumentNumber { get; set; }

        public decimal DocumentPoints { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public DocumentSeries DocumentSeries { get; set; }

        [ForeignKey(typeof(DocumentSeries))]
        [JsonProperty("DocumentSeries.Oid")]
        [Column("DocumentSeries")]
        public Guid DocumentSeriesOid { get; set; }

        public string DocumentSeriesCode { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public DocumentType DocumentType { get; set; }


        [ForeignKey(typeof(DocumentType))]
        [JsonProperty("DocumentType.Oid")]
        [Column("DocumentType")]
        public Guid DocumentTypeOid { get; set; }

        public string DocumentTypeCode { get; set; }

        public DateTime? ExecutionDate { get; set; }

        public DateTime FinalizedDate { get; set; }

        public DateTime FiscalDate { get; set; }

        public decimal GrossTotal { get; set; }

        public decimal GrossTotalBeforeDiscount { get; set; }

        public decimal GrossTotalBeforeDocumentDiscount { get; set; }

        public bool IsCanceled { get; set; }

        public bool IsNewRecord { get; set; }

        public decimal NetTotal { get; set; }

        public decimal NetTotalBeforeDiscount { get; set; }

        public string PlaceOfLoading { get; set; }

        public bool PointsAddedToCustomer { get; set; }

        public bool UsesPackingQuantities
        {
            get
            {
                return DocumentType == null ? DependencyService.Get<ICrossPlatformMethods>().GetDocumentTypes()
                                                                .Where(x => x.Oid == this.DocumentTypeOid).FirstOrDefault()?.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING :
                                                                                                                DocumentType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING;
            }

        }

        public bool PointsAddedToCustomerAtStoreController { get; set; }

        public bool PointsConsumed { get; set; }

        public bool PointsConsumedAtStoreController { get; set; }

        public decimal PointsDiscountAmount { get; set; }

        public decimal PointsDiscountPercentage { get; set; }

        public decimal PointsDiscountPercentagePerLine { get; set; }

        public int SFAID { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public PriceCatalog PriceCatalog { get; set; }

        [ForeignKey(typeof(PriceCatalog))]
        [JsonProperty("PriceCatalog.Oid")]
        [Column("PriceCatalog")]
        [JsonIgnore]
        public Guid PriceCatalogOid { get; set; }

        private PriceCatalogPolicy _PriceCatalogPolicy;

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public PriceCatalogPolicy PriceCatalogPolicy
        {
            get
            {
                return this._PriceCatalogPolicy;
            }
            set
            {
                this._EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy();
                this._PriceCatalogPolicy = value;
            }
        }

        [ForeignKey(typeof(PriceCatalogPolicy))]
        [JsonProperty("PriceCatalogPolicy.Oid")]
        [Column("PriceCatalogPolicy")]
        public Guid PriceCatalogPolicyOid { get; set; }

        public string ProcessedDenormalizedCustomer { get; set; }

        public decimal PromotionPoints { get; set; }

        public decimal PromotionsDiscountAmount { get; set; }

        public decimal PromotionsDiscountPercentage { get; set; }

        public decimal PromotionsDiscountPercentagePerLine { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public DiscountType PromotionsDocumentDiscountType { get; set; }

        [ForeignKey(typeof(DiscountType))]
        [JsonProperty("PromotionsDocumentDiscountType.Oid")]
        [Column("PromotionsDocumentDiscountType")]
        public Guid PromotionsDocumentDiscountTypeOid { get; set; }

        public DateTime RefferenceDate { get; set; }

        public string Remarks { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Store SecondaryStore { get; set; }

        [ForeignKey(typeof(Store))]
        [JsonProperty("SecondaryStore.Oid")]
        [Column("SecondaryStore")]
        public Guid SecondaryStoreOid { get; set; }

        public string Signature { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public DocumentStatus Status { get; set; }

        [ForeignKey(typeof(DocumentStatus))]
        [JsonProperty("Status.Oid")]
        [Column("Status")]
        public Guid StatusOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Store Store { get; set; }

        [ForeignKey(typeof(Store))]
        [JsonProperty("Store.Oid")]
        [Column("Store")]
        public Guid StoreOid { get; set; }

        public string StoreCode { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Supplier Supplier { get; set; }

        [ForeignKey(typeof(Supplier))]
        [JsonProperty("SupplierNew.Oid")]
        [Column("Supplier")]
        public Guid SupplierOid { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal TotalPoints { get; set; }

        public decimal TotalQty { get; set; }

        public decimal TotalVatAmount { get; set; }

        public decimal TotalVatAmountBeforeDiscount { get; set; }

        public string TransferMethod { get; set; }

        //public TransferPurpose TransferPurpose { get; set; }

        public eTransformationLevel TransformationLevel { get; set; }


        public string TriangularAddress { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Customer TriangularCustomer { get; set; }

        [ForeignKey(typeof(Customer))]
        [JsonProperty("TriangularCustomer.Oid")]
        [Column("TriangularCustomer")]
        public Guid TriangularCustomerOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Store TriangularStore { get; set; }

        [ForeignKey(typeof(Store))]
        [JsonProperty("TriangularStore.Oid")]
        [Column("TriangularStore")]
        public Guid TriangularStoreOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Supplier TriangularSupplier { get; set; }

        [ForeignKey(typeof(Supplier))]
        [JsonProperty("TriangularSupplier.Oid")]
        [Column("TriangularSupplier")]
        public Guid TriangularSupplierOid { get; set; }

        public decimal VatAmount1 { get; set; }

        public decimal VatAmount2 { get; set; }

        public decimal VatAmount3 { get; set; }

        public decimal VatAmount4 { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }


        [OneToMany(CascadeOperations = CascadeOperation.CascadeDelete)]
        public List<DocumentPayment> DocumentPayments { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeDelete)]
        public List<DocumentDetail> DocumentDetails { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeDelete)]
        public List<TransactionCoupon> TransactionCoupons { get; set; }

        [JsonIgnore]
        public ItemStockAffectionOptions ItemStockAffectionOptions { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [JsonProperty("Owner.Oid")]
        [Column("Owner")]
        [ForeignKey(typeof(CompanyNew))]
        public Guid OwnerOid { get; set; }

        public bool IsCancelingAnotherDocument
        {
            get
            {
                return this.CancelsDocumentOid != null && this.CancelsDocumentOid.Value != Guid.Empty;
            }
        }

        private EffectivePriceCatalogPolicy _EffectivePriceCatalogPolicy;

        public EffectivePriceCatalogPolicy EffectivePriceCatalogPolicy(PriceCatalogPolicy priceCatalogPolicy = null)
        {

            if (priceCatalogPolicy != null)
            {
                this._EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(this.Store, priceCatalogPolicy);
            }
            else if (this._EffectivePriceCatalogPolicy.HasPolicyDetails() == false && this.Store != null && this.PriceCatalogPolicy != null)
            {
                this._EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(this.Store, this.Customer);
            }
            return _EffectivePriceCatalogPolicy;
        }

        public DocumentHeader()
        {
            this._EffectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy();
            this.DocumentDetails = new List<DocumentDetail>();
        }

        public IEnumerable<DocumentDetail> SumarisableDocumentDetails
        {
            get
            {
                return this.DocumentDetails == null || this.DocumentDetails.Count <= 0 ? new List<DocumentDetail>() : this.DocumentDetails.Where(documentDetail => documentDetail.ShouldBeSummarised);
            }
        }


        [JsonIgnore]
        [Ignore]
        public string PrintedVat
        {
            get
            {
                try
                {
                    return this.DocumentDetails.Where(x => x.IsCanceled == false).GroupBy(d => d.VatFactor).OrderBy(x => x.Key)
                      .ToDictionary(x => x.Key, x => new
                      {
                          Vat = x.Sum(y => y.TotalVatAmount),
                          Net = x.Sum(y => y.NetTotal)
                      }).Select(x => String.Format("{0,-10:p2}{2,22:n2}{1,22:d2}", x.Key, (x.Value.Vat + "€"), (x.Value.Net + "€")))
                                .Aggregate((a, b) => String.Format("{0}\n{1}", a, b));
                }
                catch (Exception ex)
                {
                    DependencyService.Get<ICrossPlatformMethods>().LogError(ex);
                    return "";
                }
            }
        }

        [JsonIgnore]
        public string Username { get; set; }

        [JsonIgnore]
        [Ignore]
        public string PrintedPayments
        {
            get
            {
                try
                {
                    string payments = string.Empty;
                    var dict = this.DocumentPayments.GroupBy(d => d.PaymentMethod.Code).OrderBy(x => x.Key)
                           .ToDictionary(x => x.Key, x => new
                           {
                               Desc = x.FirstOrDefault().PaymentMethod.Description,
                               Amount = x.Sum(y => y.Amount)
                           });

                    foreach (var payment in dict)
                    {
                        int pad = 40 - payment.Value.Desc.Length - (8 - payment.Value.Amount.ToString().Length);
                        payments += payment.Value.Desc.PadRight(pad) + payment.Value.Amount.ToString("F") + "€" + Environment.NewLine;
                    }
                    return payments;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        public int RemoteDeviceSequence { get; set; }

        public DocumentHeader DeepCopy()
        {
            DocumentHeader other = (DocumentHeader)this.MemberwiseClone();

            return other;
        }


        public string VehicleNumber { get; set; }

        [Ignore]
        [JsonIgnore]
        public decimal Change
        {
            get
            {
                if (this.DocumentPayments != null && this.DocumentPayments.Count > 0)
                {
                    return this.DocumentPayments.Where(x => x.IsChange).Sum(x => Math.Abs(x.Amount));
                }
                return 0;
            }
        }

        [Ignore]
        [JsonIgnore]
        public bool HasChange
        {
            get
            {
                if (this.DocumentPayments != null && this.DocumentPayments.Count > 0)
                {
                    if (this.DocumentPayments.Where(x => x.IsChange).ToList().Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
