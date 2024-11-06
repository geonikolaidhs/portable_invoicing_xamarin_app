using ITS.WRM.SFA.Model.Interface;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.WRM.SFA.Model.Enumerations;

using SQLite;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using System;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 190, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentType : LookUp2Fields, IRequiredOwner
    {
        public bool AcceptsGeneralItems { get; set; }

        public bool AllowItemZeroPrices { get; set; }

        [ForeignKey(typeof(DocumentStatus))]
        [ExpandProperty]
        [JsonProperty(PropertyName = "DefaultDocumentStatus.Oid")]
        public Guid DefaultDocumentStatus { get; set; }

        public void SetDefaultDocumentStatus(DocumentStatus documentStatus)
        {
            if (documentStatus == null)
            {
                this.DefaultDocumentStatus = Guid.Empty;
            }
            else
            {
                this.DefaultDocumentStatus = documentStatus.Oid;
            }
        }

        public DocumentStatus GetDefaultDocumentStatus(DatabaseLayer databaseLayer)
        {
            if (this.DefaultDocumentStatus == Guid.Empty)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<DocumentStatus>(this.DefaultDocumentStatus);
            }
        }

        [ForeignKey(typeof(PaymentMethod))]
        [ExpandProperty]
        [JsonProperty(PropertyName = "DefaultPaymentMethod.Oid")]
        public Guid DefaultPaymentMethod { get; set; }

        public void SetDefaultPaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
            {
                this.DefaultDocumentStatus = Guid.Empty;
            }
            else
            {
                this.DefaultDocumentStatus = paymentMethod.Oid;
            }
        }

        public PaymentMethod GetDefaultPaymentMethod(DatabaseLayer databaseLayer)
        {
            if (this.DefaultPaymentMethod == Guid.Empty)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<PaymentMethod>(this.DefaultPaymentMethod);
            }
        }

        [ForeignKey(typeof(DeficiencySettings))]
        [ExpandProperty]
        [JsonProperty(PropertyName = "DeficiencySettings.Oid")]
        public Guid DeficiencySettings { get; set; }

        public void SetDeficiencySettings(DeficiencySettings deficiencySettings)
        {
            if (deficiencySettings == null)
            {
                this.DeficiencySettings = Guid.Empty;
            }
            else
            {
                this.DeficiencySettings = deficiencySettings.Oid;
            }
        }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        public Division Division { get; set; }
        [ForeignKey(typeof(Division))]
        [Column("Division")]
        [JsonProperty("Division.Oid")]
        public Guid DivisionOid { get; set; }

        public eDocTypeCustomerCategory DocTypeCustomerCategoryMode { get; set; }

        public bool DocumentHeaderCanBeCopied { get; set; }

        public eDocumentTypeItemCategory DocumentTypeItemCategoryMode { get; set; }

        public eDocumentTypeMeasurementUnit DocumentTypeProposedMeasurementUnit { get; set; }

        public string FormDescription { get; set; }

        public bool IsForWholesale { get; set; }

        public bool IsOfValues { get; set; }

        public bool IsPrintedOnStoreController { get; set; }

        public bool ManualLinkedLineInsertion { get; set; }

        public bool IsQuantitative { get; set; }

        public uint MaxCountOfLines { get; set; }

        public decimal MaxDetailQty { get; set; }

        public decimal MaxDetailTotal { get; set; }

        public decimal MaxDetailValue { get; set; }

        public decimal MaxPaymentAmount { get; set; }

        public bool MergedSameDocumentLines { get; set; }

        public bool AllowItemValueEdit { get; set; }

        [ForeignKey(typeof(MinistryDocumentType))]
        [ExpandProperty]
        [JsonProperty(PropertyName = "MinistryDocumentType.Oid")]
        public Guid MinistryDocumentType { get; set; }

        public void SetMinistryDocumentType(MinistryDocumentType ministryDocumentType)
        {
            if (ministryDocumentType == null)
            {
                this.MinistryDocumentType = Guid.Empty;
            }
            else
            {
                this.MinistryDocumentType = ministryDocumentType.Oid;
            }
        }

        public ItemStockAffectionOptions ItemStockAffectionOptions { get; set; }

        public int QuantityFactor { get; set; }

        [ForeignKey(typeof(ReasonCategory))]
        [ExpandProperty]
        [JsonProperty(PropertyName = "ReasonCategory.Oid")]
        public Guid ReasonCategory { get; set; }

        public void SetReasonCategory(ReasonCategory reasonCategory)
        {
            if (reasonCategory == null)
            {
                this.ReasonCategory = Guid.Empty;
            }
            else
            {
                this.ReasonCategory = reasonCategory.Oid;
            }
        }

        public bool RecalculatePricesOnTraderChange { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public SpecialItem SpecialItem { get; set; }

        [Column("SpecialItem")]
        [ForeignKey(typeof(SpecialItem))]
        [JsonProperty(PropertyName = "SpecialItem.Oid")]
        [Indexed]

        public Guid SpecialItemOid { get; set; }

        public bool SupportLoyalty { get; set; }

        public bool TakesDigitalSignature { get; set; }

        public eDocumentTraderType TraderType { get; set; }

        public eDocumentType Type { get; set; }

        public bool UsesPaymentMethods { get; set; }

        public bool UsesPrices { get; set; }

        public int ValueFactor { get; set; }

        public eDocumentTypeMeasurementUnit MeasurementUnitMode { get; set; }

        [Ignore]
        string IOwner.Description { get; }


        [Ignore]
        ICompanyNew IOwner.Owner
        {
            get;
        }


    }
}
