using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using SQLite;
using ITS.WRM.SFA.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 60, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class OwnerApplicationSettings : BasicObj, IOwner, IOwnerApplicationSettings
    {
        public bool AllowPriceCatalogSelection { get; set; }

        public string ApplicationTerms { get; set; }

        public bool ApplyDocumentDiscountToLinesWithoutDiscount { get; set; }

        public string B2CCompany { get; set; }

        public string B2CFAQ { get; set; }

        public string B2CProductsShipping { get; set; }

        public string B2CTransactionsSafety { get; set; }

        public string B2CUsefullInfo { get; set; }

        public int BarcodeLength { get; set; }

        public string BarcodePaddingCharacter { get; set; }

        public double ComputeDigits { get; set; }

        public double ComputeValueDigits { get; set; }

        public string Description
        {
            get;

        }

        public decimal DiscountAmount { get; set; }

        public decimal DiscountPercentage { get; set; }

        public bool DiscountPermited { get; set; }

        public double DisplayDigits { get; set; }

        public double DisplayValueDigits { get; set; }

        public decimal DocumentSumForLoyalty { get; set; }

        public string EMail { get; set; }

        public string EmailTemplateColor1 { get; set; }

        public string EmailTemplateColor2 { get; set; }

        public bool EnablePurchases { get; set; }

        public string FacebookAccount { get; set; }

        public string FAX { get; set; }

        public string Fonts { get; set; }

        public string GoogleAnalyticsID { get; set; }

        public int ItemCodeLength { get; set; }

        public string ItemCodePaddingCharacter { get; set; }

        public string LinkedInAccount { get; set; }

        public string LocationGoogleID { get; set; }

        public bool LoyaltyOnDocumentSum { get; set; }

        public Guid LoyaltyPaymentMethodOid { get; set; }

        public decimal LoyaltyPointsPerDocumentSum { get; set; }

        public eLoyaltyRefundType LoyaltyRefundType { get; set; }

        public decimal MarkupDefaultValueDifference { get; set; }

        public decimal MaximumAllowedDiscountPercentage { get; set; }

        public string MetaDescription { get; set; }


        [Column("Owner")]
        [JsonProperty(PropertyName = "Owner.Oid")]
        [ForeignKey(typeof(CompanyNew))]
        public Guid OwnerOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        public bool OnlyRefundStore { get; set; }

        public Guid OwnerImageOid { get; set; }

        public bool PadBarcodes { get; set; }

        public bool PadItemCodes { get; set; }

        public string PayPalEmail { get; set; }

        public string Phone { get; set; }

        public Guid PointsDocumentSeriesOid { get; set; }

        public Guid PointsDocumentStatusOid { get; set; }

        public Guid PointsDocumentTypeOid { get; set; }

        public ePromotionExecutionPriority PromotionExecutionPriority { get; set; }

        public bool RecomputePrices { get; set; }

        public decimal RefundPoints { get; set; }

        public string SmtpDomain { get; set; }

        public string SmtpEmailAddress { get; set; }

        public string SmtpHost { get; set; }

        public string SmtpPassword { get; set; }

        public string SmtpPort { get; set; }

        public string SmtpUsername { get; set; }

        public bool SmtpUseSSL { get; set; }

        public bool SupportLoyalty { get; set; }

        public bool TrimBarcodeOnDisplay { get; set; }

        public string TwitterAccount { get; set; }

        public bool UseBarcodeRelationFactor { get; set; }

        public bool UseMarginInsteadMarkup { get; set; }

        public string Webpage { get; set; }

        ICompanyNew IOwnerApplicationSettings.Owner
        {
            get
            {
                return this.Owner;
            }

            set
            {
                this.Owner = value as CompanyNew;
            }
        }

        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }
    }
}
