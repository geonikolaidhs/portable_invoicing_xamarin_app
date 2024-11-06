using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentType : ILookUpFields, IRequiredOwner
    {
        ePriceSuggestionType PriceSuggestionType { get; set; }
        bool DocumentHeaderCanBeCopied { get; set; }
        eDocumentType Type { get; set; }
        IDeficiencySettings DeficiencySettings { get; set; }
        bool UsesMarkUpForm { get; set; }
        bool UsesMarkUp { get; set; }
        bool IsOfValues { get; set; }
        bool IsQuantitative { get; set; }
        bool AllowItemZeroPrices { get; set; }
        IMinistryDocumentType MinistryDocumentType { get; set; }
        bool IsForWholesale { get; set; }
        int ValueFactor { get; set; }
        int QuantityFactor { get; set; }
        bool UsesPrices { get; set; }
        bool TakesDigitalSignature { get; set; }
        bool MergedSameDocumentLines { get; set; }
        IDivision Division { get; set; }
        bool UsesPaymentMethods { get; set; }
        bool SupportLoyalty { get; set; }
        eDocumentTypeMeasurementUnit DocumentTypeProposedMeasurementUnit { get; set; }
        IDocumentStatus DefaultDocumentStatus { get; set; }
        IPaymentMethod DefaultPaymentMethod { get; set; }
        string FormDescription { get; set; }
        bool RecalculatePricesOnTraderChange { get; set; }
        eDocTypeCustomerCategory DocTypeCustomerCategoryMode { get; set; }
        eDocumentType
            DocumentTypeItemCategoryMode
        { get; set; }
        bool ShouldResetMenu { get; set; }
        uint MaxCountOfLines { get; set; }
        bool AcceptsGeneralItems { get; set; }
        bool ReserveCoupons { get; set; }
        bool IsPrintedOnStoreController { get; set; }
        decimal MaxDetailQty { get; set; }
        decimal MaxDetailValue { get; set; }
        decimal MaxDetailTotal { get; set; }
        decimal MaxPaymentAmount { get; set; }
        IReasonCategory ReasonCategory { get; set; }
        eDocumentTraderType TraderType { get; set; }
        ISpecialItem SpecialItem { get; set; }
    }
}
