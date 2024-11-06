using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentHeader
    {
        string DenormalizedCustomer { get; set; }
        bool HasBeenOnHold { get; set; }
        IStore SecondaryStore { get; set; }
        //IDocumentSource Source { get; set; }
        eTransformationLevel TransformationLevel { get; set; }
        DateTime? ExecutionDate { get; set; }
        IAddress BillingAddress { get; set; }
        decimal GrossTotalBeforeDocumentDiscount { get; set; }
        DateTime RefferenceDate { get; set; }
        IPriceCatalog PriceCatalog { get; set; }
        ISupplierNew Supplier { get; set; }
        ITransferPurpose TransferPurpose { get; set; }
        string DocumentStatusCode { get; set; }
        string DeliveryToTraderTaxCode { get; set; }
        string CustomerCode { get; set; }
        string StoreCode { get; set; }
        string DocumentSeriesCode { get; set; }
        string DocumentTypeCode { get; set; }
        int POSID { get; set; }
        string PlaceOfLoading { get; set; }
        string TransferMethod { get; set; }
        string Signature { get; set; }
        IPOS POS { get; set; }
        bool IsCanceled { get; set; }
        bool HasBeenExecuted { get; set; }
        bool HasBeenChecked { get; set; }
        bool DocumentFinished { get; set; }
        bool CheckFromStore { get; set; }
        bool IsNewRecord { get; set; }
        //IUserDailyTotals UserDailyTotals { get; set; }
        Guid? CancelsDocumentOid { get; set; }
        bool IsFiscalPrinterHandled { get; set; }
        decimal TotalQty { get; set; }
        bool HasPaymentWithRatification { get; set; }
        decimal TotalPoints { get; set; }
        decimal TotalVatAmountBeforeDiscount { get; set; }
        decimal GrossTotalBeforeDiscount { get; set; }
        decimal NetTotalBeforeDiscount { get; set; }
        IDiscountType DocumentDiscountType { get; set; }
        bool PointsConsumed { get; set; }
        bool PointsAddedToCustomer { get; set; }
        bool PointsConsumedAtStoreController { get; set; }
        bool PointsAddedToCustomerAtStoreController { get; set; }
        decimal DocumentPoints { get; set; }
        DateTime FiscalDate { get; set; }
        DateTime InvoicingDate { get; set; }
        IDocumentType DocumentType { get; set; }
        IDocumentSeries DocumentSeries { get; set; }
        Guid? CanceledByDocumentOid { get; set; }
        decimal PromotionPoints { get; set; }
        string DeliveryAddress { get; set; }
        ITrader DeliveryTo { get; set; }

        int DocumentNumber { get; set; }
        IStore Store { get; set; }
        IDivision Division { get; set; }
        ICustomer Customer { get; set; }
        decimal DocumentDiscountPercentage { get; set; }
        decimal DocumentDiscountPercentagePerLine { get; set; }
        decimal DocumentDiscountAmount { get; set; }
        decimal PromotionsDiscountAmount { get; set; }
        decimal PromotionsDiscountPercentagePerLine { get; set; }
        decimal PromotionsDiscountPercentage { get; set; }
        decimal PointsDiscountAmount { get; set; }
        decimal PointsDiscountPercentagePerLine { get; set; }
        decimal ConsumedPointsForDiscount { get; set; }
        decimal PointsDiscountPercentage { get; set; }
        decimal VatAmount1 { get; set; }
        decimal VatAmount2 { get; set; }
        IDocumentStatus Status { get; set; }
        decimal GrossTotal { get; set; }
        DateTime FinalizedDate { get; set; }
        decimal VatAmount3 { get; set; }
        decimal VatAmount4 { get; set; }
        decimal TotalDiscountAmount { get; set; }
        string Remarks { get; set; }
        decimal TotalVatAmount { get; set; }
        decimal NetTotal { get; set; }
        bool IsShiftStartingAmount { get; set; }
        bool IsDayStartingAmount { get; set; }
        bool CustomerNotFound { get; set; }
        string CustomerLookupCode { get; set; }
        string CustomerName { get; set; }
        ICustomer TriangularCustomer { get; set; }
        ISupplierNew TriangularSupplier { get; set; }
        IStore TriangularStore { get; set; }
        bool InEmulationMode { get; set; }
        bool CouponsHaveBeenUpdatedOnStoreController { get; set; }
        bool CouponsHaveBeenUpdatedOnMaster { get; set; }
        IDiscountType PromotionsDocumentDiscountType { get; set; }
        //IInsertedCustomerViewModel CustomerViewModel { get; set; }
        string CustomerLookUpTaxCode { get; set; }
        string ProcessedDenormalizedCustomer { get; set; }
        Guid? DenormalisedAddress { get; set; }
        string AddressProfession { get; set; }
        string TriangularAddress { get; set; }
        IPriceCatalogPolicy PriceCatalogPolicy { get; set; }
    }
}
