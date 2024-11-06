using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IOwnerApplicationSettings: IBasicObj
    {
         Guid PointsDocumentSeriesOid { get; set; }
         Guid PointsDocumentTypeOid { get; set; }
        string Fonts { get; set; }
        //IPayPalMode PayPalMode { get; set; }
        string PayPalEmail { get; set; }
        string B2CProductsShipping { get; set; }
        string B2CTransactionsSafety { get; set; }
        string B2CCompany { get; set; }
        string B2CUsefullInfo { get; set; }
        string B2CFAQ { get; set; }
        string MetaDescription { get; set; }
        string GoogleAnalyticsID { get; set; }
        string SmtpHost { get; set; }
        string SmtpPort { get; set; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string SmtpDomain { get; set; }
        string SmtpEmailAddress { get; set; }
        bool SmtpUseSSL { get; set; }
        string EmailTemplateColor1 { get; set; }
        string EmailTemplateColor2 { get; set; }
        eLoyaltyRefundType LoyaltyRefundType { get; set; }
        decimal MaximumAllowedDiscountPercentage { get; set; }
        string FAX { get; set; }
        bool OnlyRefundStore { get; set; }
        string Phone { get; set; }
        string LocationGoogleID { get; set; }
        string LinkedInAccount { get; set; }
        string FacebookAccount { get; set; }
        string TwitterAccount { get; set; }
        string Webpage { get; set; }
        string EMail { get; set; }
        bool TrimBarcodeOnDisplay { get; set; }
        ICompanyNew Owner { get; set; }
        double DisplayValueDigits { get; set; }
        double ComputeValueDigits { get; set; }
        int BarcodeLength { get; set; }
        int ItemCodeLength { get; set; }
        string BarcodePaddingCharacter { get; set; }
        string ItemCodePaddingCharacter { get; set; }
        double ComputeDigits { get; set; }
        double DisplayDigits { get; set; }
        Guid OwnerImageOid { get; set; }
        bool RecomputePrices { get; set; }
        bool DiscountPermited { get; set; }
        string ApplicationTerms { get; set; }
        bool AllowPriceCatalogSelection { get; set; }
        decimal RefundPoints { get; set; }
        decimal DiscountAmount { get; set; }
        decimal DiscountPercentage { get; set; }
        bool SupportLoyalty { get; set; }
        bool UseBarcodeRelationFactor { get; set; }
        bool ApplyDocumentDiscountToLinesWithoutDiscount { get; set; }
        bool LoyaltyOnDocumentSum { get; set; }
        decimal DocumentSumForLoyalty { get; set; }
        decimal LoyaltyPointsPerDocumentSum { get; set; }
        Guid LoyaltyPaymentMethodOid { get; set; }
        bool EnablePurchases { get; set; }
        bool PadItemCodes { get; set; }
        bool PadBarcodes { get; set; }
        ePromotionExecutionPriority PromotionExecutionPriority { get; set; }
        Guid PointsDocumentStatusOid { get; set; }
        decimal MarkupDefaultValueDifference { get; set; }
        bool UseMarginInsteadMarkup { get; set; }

        
    }
}
