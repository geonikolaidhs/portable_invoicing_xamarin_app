using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public static class ItemHelper
    {

        /// <summary>
        /// Returns all the points corresponding to an Item by suming recursively the points of all Item's ItemCategories
        /// </summary>
        /// <param name="item">The Item</param>
        /// <param name="type">The document type</param>
        /// <param name="priceCatalog">The price catalog</param>
        /// <returns>The sum of Points</returns>
        public static decimal GetPointsOfItem(DatabaseLayer databaseLayer, Item item, DocumentType type, OwnerApplicationSettings settings)
        {
            if (type.SupportLoyalty && settings.SupportLoyalty)
            {
                decimal points = item.Points;
                List<ItemAnalyticTree> ItemAnalyticTrees = databaseLayer.GetItemAnalyticTrees(item.Oid);
                foreach (ItemAnalyticTree iat in ItemAnalyticTrees)
                {
                    if (iat.Node != null)
                    {
                        ItemCategory CurrentCategory = databaseLayer.GetItemCategoryById(iat.NodeOid);
                        while (CurrentCategory != null)
                        {
                            points += CurrentCategory.Points;
                            if (CurrentCategory.ParentOid.HasValue)
                            {
                                CurrentCategory = databaseLayer.GetItemCategoryById(CurrentCategory.ParentOid.Value);
                            }
                            else
                            {
                                CurrentCategory = null;
                            }
                        }
                    }
                }
                return points;
            }
            return 0;
        }


        public static PriceCatalogDetail GetPriceCatalogDetail(DatabaseLayer databaseLayer, Guid ItemOid, PriceCatalog priceCatalog, Guid barcodeOid, string itemCode, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            try
            {
                DateTime now = DateTime.Now;
                PriceCatalogDetail priceCatalogDetail = null;
                if (traces != null)
                {
                    traces.Add(new PriceSearchTraceStep()
                    {
                        PriceCatalogDescription = priceCatalog.Description,
                        SearchMethod = priceCatalogSearchMethod,
                        Number = traces.Count + 1
                    });
                }
                if (barcodeOid != null && barcodeOid != Guid.Empty)
                {
                    priceCatalogDetail = databaseLayer.GetPriceCatalogDetail(priceCatalog, barcodeOid, ItemOid);
                }
                if (priceCatalogDetail == null || (priceCatalog.IgnoreZeroPrices && priceCatalogDetail.Value == 0))
                {
                    OwnerApplicationSettings ownAppSet = App.OwnerApplicationSettings;
                    string paddedCode = (ownAppSet.PadBarcodes) ? itemCode.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : itemCode;
                    Barcode fkBarcode = databaseLayer.GetBarcodeByCode(paddedCode);  // Do Padding
                    if (fkBarcode == null)
                    {
                        return null;
                    }
                    priceCatalogDetail = databaseLayer.GetPriceCatalogDetail(priceCatalog, fkBarcode.Oid, ItemOid);
                    if (priceCatalogDetail == null || (priceCatalog.IgnoreZeroPrices && priceCatalogDetail.Value == 0))
                    {
                        PriceCatalog parentPriceCatalog = App.PriceCatalogs.Where(x => x.Oid == priceCatalog.ParentCatalogOid).FirstOrDefault();
                        if ((!priceCatalog.IsRoot || parentPriceCatalog != null)
                            && priceCatalogSearchMethod == PriceCatalogSearchMethod.PRICECATALOG_TREE
                          )
                        {
                            return GetPriceCatalogDetail(databaseLayer, ItemOid, parentPriceCatalog, barcodeOid, itemCode, traces: traces);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return priceCatalogDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static VatFactor GetItemVatFactor(Guid VatCategoryOid)
        {
            return App.VatFactors.Where(x => x.VatLevelOid == App.StoreVatLevel.Oid && x.VatCategoryOid == VatCategoryOid).FirstOrDefault();
        }

        public static ItemBarcode GetTaxCodeBarcode(DatabaseLayer databaseLayer, Item item, CompanyNew owner, OwnerApplicationSettings ownerApplicationSettings, Barcode detailBarcode)
        {
            if (item == null || owner == null)
            {
                throw new Exception(string.Format(ResourcesRest.ItemAndOwnerMustBeDefined, item == null ? "" : item.Name, owner == null ? "" : owner.CompanyName));
            }

            string barcodeCode = item.Code;
            if (ownerApplicationSettings != null && ownerApplicationSettings.PadBarcodes && string.IsNullOrEmpty(ownerApplicationSettings.BarcodePaddingCharacter) == false)
            {
                barcodeCode = item.Code.PadLeft(ownerApplicationSettings.BarcodeLength, ownerApplicationSettings.BarcodePaddingCharacter[0]);
            }
            Barcode taxCodeBarcode = null;
            if (detailBarcode != null && item.DefaultBarcode != null && item.DefaultBarcode.Oid == detailBarcode.Oid && detailBarcode.Code == barcodeCode)
            {
                return detailBarcode.ItemBarcode(databaseLayer, owner);
            }
            if (taxCodeBarcode == null)
            {
                taxCodeBarcode = databaseLayer.GetTaxCodeBarcode(barcodeCode);
            }
            if (taxCodeBarcode == null)
            {
                if (item.DefaultBarcode == null)
                {
                    item.DefaultBarcode = databaseLayer.GetBarcodeById(item.DefaultBarcodeOid);
                }
                return item.DefaultBarcode.ItemBarcode(databaseLayer, owner);
            }
            return databaseLayer.GetItemBarcode(item, taxCodeBarcode, owner);
        }

        public static Item GetItemFromDetail(DocumentDetail detail)
        {
            if (detail.Item != null)
            {
                return detail.Item;
            }
            return App.DbLayer.GetItem(detail.ItemOid);
        }

        public static List<DocumentDetail> CheckStockBeforeSendDocument(DocumentHeader document)
        {
            List<DocumentDetail> outOfStockDetails = new List<DocumentDetail>();
            if (document.DocumentDetails == null)
            {
                document.DocumentDetails = App.DbLayer.LoadDocumentFromDatabase(document.Oid).DocumentDetails;
                if (document.DocumentDetails == null || document.DocumentDetails.Count == 0)
                {
                    return outOfStockDetails;
                }
            }
            foreach (DocumentDetail dtl in document.DocumentDetails)
            {
                if (dtl.Item == null)
                {
                    dtl.Item = ItemHelper.GetItemFromDetail(dtl);
                }
                if (dtl.Item != null)
                {
                    double insertedQty;
                    if (double.TryParse(dtl.Qty.ToString(), out insertedQty))
                    {
                        if (insertedQty > dtl.Item.Stock)
                            outOfStockDetails.Add(dtl);
                    }
                }
            }
            return outOfStockDetails;
        }

        public static bool CheckItemStockBeforeInsertToDocument(Item item, DocumentHeader document, decimal requestedQuantity, out double availiableStock)
        {
            availiableStock = item.Stock;
            if (document.DocumentType == null)
            {
                document.DocumentType = App.DocumentTypes.Where(x => x.Oid == document.DocumentTypeOid).FirstOrDefault();
            }
            if (document.DocumentType != null && document.DocumentType.ItemStockAffectionOptions == Model.Enumerations.ItemStockAffectionOptions.AFFECTS && document.DocumentType.QuantityFactor > 0)
            {
                decimal documentQuantity = document.DocumentDetails.Where(x => x.ItemOid == item.Oid).Sum(z => z.Qty);
                availiableStock = item.Stock - (double)documentQuantity;
                return availiableStock >= (double)requestedQuantity ? true : false;
            }
            return true;
        }

        public static bool CheckItemStockOnUpdateToDocument(Item item, DocumentHeader document, decimal requestedQuantity, out double availiableStock)
        {
            availiableStock = item.Stock;
            if (document.DocumentType == null)
            {
                document.DocumentType = App.DocumentTypes.Where(x => x.Oid == document.DocumentTypeOid).FirstOrDefault();
            }
            if (document.DocumentType != null && document.DocumentType.ItemStockAffectionOptions == Model.Enumerations.ItemStockAffectionOptions.AFFECTS && document.DocumentType.QuantityFactor > 0)
            {
                return availiableStock >= (double)requestedQuantity ? true : false;
            }
            return true;
        }


        public static bool CheckItemStock(Item item, decimal qty)
        {
            double requestedQuantity;
            if (double.TryParse(qty.ToString(), out requestedQuantity))
            {
                if (requestedQuantity <= item.Stock)
                    return true;
            }
            return false;
        }

        public static void UpdateStockOnCloseDocument(Dictionary<Guid, double> details, DatabaseLayer dbLayer, DocumentType docType)
        {
            foreach (KeyValuePair<Guid, double> dtl in details)
            {
                try
                {
                    Item item = dbLayer.GetById<Item>(dtl.Key);
                    if (item != null)
                    {
                        item.Stock = item.Stock - (docType.QuantityFactor * dtl.Value);
                        dbLayer.InsertOrReplaceObj<Item>(item);
                    }

                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }
            }
        }


        public static bool HasPackingMeasurementunit(DocumentType docType, MeasurementUnit packingMeasurementunit, MeasurementUnit baseMeasurementunit, double packingQty)
        {
            if (docType == null || packingMeasurementunit == null || baseMeasurementunit == null)
            {
                return false;
            }
            if (docType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING && packingMeasurementunit != baseMeasurementunit && packingQty > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static decimal GetPackingQuantity(decimal qty, double packingMeasurementUnitRelationFactor)
        {
            if (qty == 0)
                return 0;
            if (packingMeasurementUnitRelationFactor == 0)
                return qty;

            return Math.Round((qty / (decimal)packingMeasurementUnitRelationFactor), 3, MidpointRounding.AwayFromZero);
        }

        public static decimal GetQuantityFromPacking(decimal qty, double packingMeasurementUnitRelationFactor)
        {
            if (qty == 0)
                return 0;
            if (packingMeasurementUnitRelationFactor == 0)
                return qty;

            return Math.Round((qty * (decimal)packingMeasurementUnitRelationFactor), 3, MidpointRounding.AwayFromZero);
        }

        public static string GetMeasurementUnitDescription(DocumentType docType, Guid packingMeasurementUnitOid, Guid baseMeasurementUnitOid, double PackingQty, decimal qty,
                                                                                                                    DatabaseLayer dbLayer, out bool hasPackingMeasurementunit)
        {
            hasPackingMeasurementunit = false;
            string desc = string.Empty;
            if (docType == null)
            {
                return desc;
            }
            if (packingMeasurementUnitOid != null && packingMeasurementUnitOid != Guid.Empty && baseMeasurementUnitOid != null && baseMeasurementUnitOid != Guid.Empty && baseMeasurementUnitOid != packingMeasurementUnitOid)
            {
                MeasurementUnit packingMeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == packingMeasurementUnitOid).FirstOrDefault();
                MeasurementUnit baseMeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == baseMeasurementUnitOid).FirstOrDefault();

                if (docType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.PACKING && packingMeasurementUnit != null && baseMeasurementUnit != null && PackingQty > 0)
                {
                    if (decimal.TryParse(PackingQty.ToString(), out decimal packQty))
                    {
                        hasPackingMeasurementunit = true;
                        desc = qty + " " + packingMeasurementUnit.Description + " ~ " + packQty + " " + baseMeasurementUnit.Description;
                    }
                }
            }
            if (docType.MeasurementUnitMode == eDocumentTypeMeasurementUnit.DEFAULT || desc == string.Empty)
            {
                desc = App.MeasurementUnits.Where(x => x.Oid == baseMeasurementUnitOid).FirstOrDefault()?.Description ?? "";
            }
            return desc;
        }

        public static decimal GetCurrentDocumentItemQuantity(Guid itemOid, List<DocumentDetail> details)
        {
            decimal val = 0;
            val = details.Where(x => x.ItemOid == itemOid).Sum(x => x.Qty);
            return val;
        }

        public static decimal GetItemQuantityFromTemporaryList(Guid itemOid, List<ItemDetail> details)
        {
            decimal val = 0;
            val = details.Where(x => x.ItemOid == itemOid).Sum(x => x.TotalQty);
            return val;
        }


    }
}