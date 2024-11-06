using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentDetail : IBaseObj
    {
        IMeasurementUnit PackingMeasurementUnit { get; set; }
        IStore CentralStore { get; set; }
        Guid LinkedLine { get; set; }
        IItem Item { get; set; }
        ISpecialItem SpecialItem { get; set; }
        IMeasurementUnit MeasurementUnit { get; set; }
        IBarcode Barcode { get; set; }
        IDocumentHeader DocumentHeader { get; set; }
        double PackingMeasurementUnitRelationFactor { get; set; }
        decimal PackingQuantity { get; set; }
        int LineNumber { get; set; }
        decimal GrossTotalBeforeDocumentDiscount { get; set; }
        decimal GrossTotalDeviation { get; set; }
        decimal TotalVatAmountDeviation { get; set; }
        decimal NetTotalDeviation { get; set; }
        Guid VatFactorGuid { get; set; }
        decimal CustomUnitPrice { get; set; }
        string CustomMeasurementUnit { get; set; }
        //decimal _GrossTotalAfterFirstDiscount;
        string Remarks { get; set; }
        string ItemCode { get; set; }
        string VatFactorCode { get; set; }
        string MeasurementUnitCode { get; set; }
        string BarcodeCode { get; set; }
        decimal GrossTotalBeforeDiscount { get; set; }
        bool HasCustomDescription { get; set; }
        bool HasCustomPrice { get; set; }
        string CustomDescription { get; set; }
        bool IsCanceled { get; set; }
        decimal PriceListUnitPrice { get; set; }
        decimal UnitPrice { get; set; }
        decimal Qty { get; set; }
        decimal Points { get; set; }
        decimal VatFactor { get; set; }
        decimal GrossTotal { get; set; }
        decimal FinalUnitPrice { get; set; }
        decimal TotalDiscount { get; set; }
        decimal TotalVatAmount { get; set; }
        decimal NetTotal { get; set; }
        int IsOffer { get; set; }
        string OfferDescription { get; set; }
        bool IsReturn { get; set; }
        decimal TotalVatAmountBeforeDiscount { get; set; }
        decimal NetTotalBeforeDiscount { get; set; }
        string _easurementUnit2Code { get; set; }
        decimal FirstDiscount { get; set; }
        decimal SecondDiscount { get; set; }
        Guid DocumentHeaderOid { get; set; }
        bool FromScanner { get; set; }
        string POSGeneratedPriceCatalogDetailSerialized { get; set; }
        bool IsPOSGeneratedPriceCatalogDetailApplied { get; set; }
        decimal TotalDiscountAmountWithVAT { get; set; }
        decimal TotalDiscountAmountWithoutVAT { get; set; }
        decimal CurrentPromotionDiscountValue { get; set; }
        decimal PromotionsLineDiscountsAmount { get; set; }
        string _ithdrawDepositTaxCode { get; set; }
        IReason Reason { get; set; }
        decimal PriceListUnitPriceWithVAT { get; set; }
        decimal PriceListUnitPriceWithoutVAT { get; set; }
    }
}
