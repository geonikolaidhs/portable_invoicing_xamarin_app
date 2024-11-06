using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public static class DocumentHelper
    {

        /// <summary>
        /// Generates o new DocumentHeader
        /// </summary>
        /// <param name="databaseLayer"></param>
        /// <param name="selectedCustomer"></param>
        /// <param name="sfaDocumentType"></param>
        /// <returns></returns>
        public static DocumentHeader GenareteDocumentHeader(DatabaseLayer databaseLayer, Customer selectedCustomer, DocumentType docType, Address selectedAddress, Store secondaryStore)
        {
            try
            {
                Store store = App.Store;
                List<StorePriceList> PriceList = App.StorePriceList;
                DocumentSeries documentSeries = databaseLayer.GetSerieByDocTypeAndStore(store.Oid, docType.Oid);
                PriceCatalogPolicy priceCatalogPolicy = PriceCatalogHelper.GetPriceCatalogPolicy(databaseLayer, store, selectedCustomer);
                DocumentStatus status = App.DocumentStatuses.Where(x => x.Oid == App.SFASettings.DefaultDocumentStatusOid).FirstOrDefault();
                Trader trader = selectedCustomer.Trader == null ? databaseLayer.GetTraderById(selectedCustomer.TraderOid) : selectedCustomer.Trader;
                Division division = App.DbLayer.GetById<Division>(docType.DivisionOid);
                string deviceOid = App.DbLayer.GetAllSfaDevices().Where(x => x.ID == App.SFASettings.SfaId).FirstOrDefault()?.Oid.ToString() ?? "";
                if (store != null && docType != null && documentSeries != null && priceCatalogPolicy != null && selectedCustomer != null && status != null)
                {
                    DocumentHeader documentHeader = new DocumentHeader()
                    {
                        CustomerOid = selectedCustomer.Oid,
                        CustomerCode = selectedCustomer.Code,
                        Customer = selectedCustomer,
                        CustomerName = selectedCustomer.CompanyName,
                        CustomerLookUpTaxCode = trader?.TaxCode ?? "",
                        DocumentType = docType,
                        ItemStockAffectionOptions = docType.ItemStockAffectionOptions,
                        DocumentSeries = documentSeries,
                        DocumentSeriesOid = documentSeries.Oid,
                        DocumentTypeOid = docType.Oid,
                        Division = division?.Section ?? eDivision.Sales,
                        Store = store,
                        StoreOid = store.Oid,
                        StoreCode = store.Code,
                        Owner = store.Owner,
                        OwnerOid = store.OwnerOid,
                        PriceCatalogPolicy = priceCatalogPolicy,
                        PriceCatalogPolicyOid = priceCatalogPolicy.Oid,
                        DocumentTypeCode = docType.Code,
                        DocumentSeriesCode = documentSeries.Code,
                        Status = status,
                        StatusOid = status?.Oid ?? Guid.Empty,
                        BillingAddress = selectedAddress,
                        BillingAddressOid = selectedAddress?.Oid ?? Guid.Empty,
                        AddressProfession = selectedAddress?.Profession ?? "",
                        CreatedBy = App.UserId,
                        UpdatedBy = App.UserId,
                        UpdatedOnTicks = DateTime.Now.Ticks,
                        CreatedOnTicks = DateTime.Now.Ticks,
                        IsActive = true,
                        IsNewRecord = true,
                        IsSynchronized = false,
                        DeliveryAddress = selectedAddress?.City + " " + selectedAddress?.Street + " " + selectedAddress?.PostCode + " " + selectedAddress?.PostCode,
                        CreatedByDevice = deviceOid,
                        UpdateByDevice = deviceOid,
                        SFAID = App.SFASettings.SfaId,
                        VehicleNumber = App.SFASettings?.VehicleNumber,
                        Username = App.UserName
                    };
                    if (secondaryStore != null)
                    {
                        documentHeader.SecondaryStore = secondaryStore;
                        documentHeader.SecondaryStoreOid = secondaryStore.Oid;
                    }
                    if (secondaryStore == null && division.Section == eDivision.Store)
                    {
                        documentHeader.SecondaryStore = App.Store;
                        documentHeader.SecondaryStoreOid = App.Store.Oid;
                    }
                    return documentHeader;
                }
                else
                {
                    throw new Exception("Wrong parameters for DocumentType");
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw ex;
            }
        }

        public static DocumentDetail ComputeDocumentLine(ref DocumentHeader documentHeader, DatabaseLayer databaseLayer, Item item, Barcode barcode, decimal qty,
        bool is_linked_line, decimal unitPrice, bool hasCustomPrice, string customDescription, List<DocumentDetailDiscount> discounts,
        bool hasCustomMeasurementUnit = false, string customMeasurementUnit = "", DocumentDetail oldDocumentLine = null, VatFactor customVatFactor = null)
        {
            bool isNew = false;
            DocumentDetail currentLine;
            if (oldDocumentLine != null)
            {
                currentLine = oldDocumentLine;
                isNew = false;
            }
            else
            {
                currentLine = new DocumentDetail();
                isNew = true;
            }
            currentLine.DocumentHeader = documentHeader;
            currentLine.DocumentHeaderOid = documentHeader.Oid;
            databaseLayer.InsertOrReplaceObj<DocumentDetail>(currentLine);
            decimal price;  // -> PriceListUnitPrice
            bool vatIncluded;
            if (qty <= 0)//&& !documentHeader.IsCancelingAnotherDocument)
            {
                qty = 1;
            }
            OwnerApplicationSettings OwnerApplicationSettings = App.OwnerApplicationSettings;
            currentLine.ItemName = item.Name;
            //Adding or replacing detail's discounts
            if (discounts != null && discounts.Count > 0)
            {
                foreach (DocumentDetailDiscount discount in discounts.ToList())
                {
                    DocumentDetailDiscount DiscountToRemove = currentLine.DocumentDetailDiscounts.Where(x => x.DiscountSource == discount.DiscountSource).FirstOrDefault();
                    if (DiscountToRemove != null)
                    {
                        currentLine.DocumentDetailDiscounts.Remove(DiscountToRemove);
                        databaseLayer.DeleteObj<DocumentDetailDiscount>(DiscountToRemove);
                    }
                    if (discount.Value != 0 || discount.Percentage != 0)
                    {
                        currentLine.DocumentDetailDiscounts.Add(discount);
                        databaseLayer.InsertOrReplaceObj<DocumentDetailDiscount>(discount);
                    }
                }
            }
            try
            {
                // Fill in basic details: Item, barcode & description
                currentLine.Item = item;
                currentLine.ItemCode = item == null ? "" : item.Code;
                currentLine.ItemOid = item.Oid;
                Address documentHeaderBillingAddress = null;
                currentLine.Barcode = barcode;
                currentLine.BarcodeOid = barcode?.Oid ?? Guid.Empty;
                currentLine.BarcodeCode = barcode == null ? "" : barcode.Code;
                if (currentLine.Item.AcceptsCustomDescription && !String.IsNullOrEmpty(customDescription))
                {
                    currentLine.CustomDescription = customDescription;
                }
                else
                {
                    currentLine.CustomDescription = item.Name;
                }
                currentLine.BarcodeOid = barcode == null ? Guid.Empty : barcode.Oid;

                VatFactor vatFactor = customVatFactor;

                if (documentHeader.Store == null)
                {
                    documentHeader.Store = App.Store;
                }
                if (documentHeader.Store.Address == null)
                {
                    Address addr = databaseLayer.GetAddressById(documentHeader.Store.AddressOid);
                    documentHeader.Store.Address = addr;
                }
                if (documentHeader.BillingAddress == null)
                {
                    documentHeader.BillingAddress = databaseLayer.GetAddressById(documentHeader.BillingAddressOid);
                }
                if (documentHeader.DocumentType == null)
                {
                    Guid docTypeOid = documentHeader.DocumentTypeOid;
                    documentHeader.DocumentType = App.DocumentTypes.Where(x => x.Oid == docTypeOid).FirstOrDefault();
                }
                if (documentHeader.Owner == null)
                {
                    documentHeader.Owner = App.Owner;
                }
                if (documentHeader.Customer == null)
                {
                    documentHeader.Customer = App.DbLayer.GetCustomer(documentHeader.CustomerOid);
                }
                documentHeader.AddressProfession = documentHeader.BillingAddress?.Profession;
                documentHeaderBillingAddress = documentHeader.BillingAddress;

                if (vatFactor == null)
                {
                    VatCategory vatcat = App.VatCategories.Where(x => x.Oid == item.VatCategoryOid).FirstOrDefault();
                    item.VatCategory = vatcat;
                    VatLevel vatlev;
                    switch (documentHeader.Division)
                    {
                        case eDivision.Sales:
                            vatlev = documentHeader.Customer.GetVatLevel(documentHeader.BillingAddress);
                            break;
                        case eDivision.Purchase:
                            if (documentHeader.Supplier == null && documentHeader.Supplier != null && documentHeader.SupplierOid != Guid.Empty)
                            {
                                documentHeader.Supplier = App.DbLayer.GetById<Supplier>(documentHeader.SupplierOid);
                            }
                            if (documentHeader.Supplier == null)
                            {
                                Guid storeAddrOid = documentHeader.Store.Address.VatLevelOid;
                                vatlev = App.VatLevels.Where(x => x.Oid == storeAddrOid).FirstOrDefault();
                            }
                            else
                            {
                                vatlev = documentHeader.Supplier.GetVatLevel(documentHeader.BillingAddress);
                            }
                            break;
                        case eDivision.Store:
                            Guid addrOid = documentHeader.Store.Address.VatLevelOid;
                            vatlev = App.VatLevels.Where(x => x.Oid == addrOid).FirstOrDefault();
                            if (vatlev == null)
                            {
                                throw new Exception("VatLevel For Store is not defined");
                            }
                            break;
                        case eDivision.Other:
                            Guid addrrOid = documentHeader.Store.Address.VatLevelOid;
                            vatlev = App.VatLevels.Where(x => x.Oid == addrrOid).FirstOrDefault();
                            if (vatlev == null)
                            {
                                throw new Exception("VatLevel For Store is not defined");
                            }
                            break;
                        default:
                            throw new Exception("Wrong Division");
                    }
                    if (item.VatCategory == null)
                    {
                        vatcat = App.DefaultVatCategory;
                        if (vatcat == null)
                        {
                            throw new Exception(ResourcesRest.ItemVatCategoryNotFound + System.Environment.NewLine + currentLine.ToString());
                        }
                    }
                    if (vatlev == null)
                    {

                        vatlev = App.VatLevels.Where(x => x.IsDefault).FirstOrDefault();
                        if (vatlev == null)
                        {
                            throw new Exception(ResourcesRest.CustomerVatLevelNotFound + System.Environment.NewLine + currentLine.ToString());
                        }
                    }

                    vatFactor = App.VatFactors.Where(x => x.VatCategoryOid == vatcat.Oid && x.VatLevelOid == vatlev.Oid).FirstOrDefault();
                    if (vatFactor == null)
                    {
                        throw new Exception(ResourcesRest.VatFactorNotFound + System.Environment.NewLine + currentLine.ToString());
                    }
                }
                Guid documentVatLevelOid = documentHeader.Store.Address.VatLevelOid;

                VatFactor oldVatFactor = oldDocumentLine != null && currentLine.VatFactorGuid != Guid.Empty ? App.VatFactors.Where(x => x.Oid == currentLine.VatFactorGuid).FirstOrDefault()
                                                               : App.VatFactors.Where(x => x.VatCategoryOid == item.VatCategoryOid && x.VatLevelOid == documentVatLevelOid).FirstOrDefault();

                if (oldVatFactor == null)
                {
                    throw new Exception(ResourcesRest.VatFactorNotFound + System.Environment.NewLine + currentLine.ToString());
                }

                currentLine.VatFactor = vatFactor.Factor;
                currentLine.VatFactorCode = vatFactor.Code;
                currentLine.VatFactorGuid = vatFactor.Oid;
                currentLine.ItemVatCategoryDescription = String.Format("{0,6:#0.0 %}", currentLine.VatFactor);

                //Measurement Unit
                Guid mm;
                if (Guid.TryParse(customMeasurementUnit, out mm))
                {
                    currentLine.MeasurementUnit = databaseLayer.GetById<MeasurementUnit>(mm);
                }
                if (currentLine.MeasurementUnit == null)
                {
                    if (barcode == null)
                    {
                        Barcode bc = databaseLayer.GetBarcodeById(item.DefaultBarcodeOid);
                        item.DefaultBarcode = bc;
                        currentLine.Barcode = bc;
                        if (documentHeader.Owner == null)
                        {
                            documentHeader.Owner = App.Owner;
                        }
                        currentLine.MeasurementUnit = bc.MeasurementUnit(databaseLayer, documentHeader.Owner);
                    }
                    else
                    {
                        currentLine.MeasurementUnit = barcode.MeasurementUnit(databaseLayer, documentHeader.Owner);
                        currentLine.Barcode = barcode;
                    }
                }

                currentLine.MeasurementUnitOid = currentLine.MeasurementUnit?.Oid ?? Guid.Empty;

                if (currentLine.Barcode == null && currentLine.BarcodeOid != null && currentLine.BarcodeOid != Guid.Empty)
                {
                    currentLine.Barcode = databaseLayer.GetBarcodeById(currentLine.BarcodeOid);
                }
                PriceCatalogDetail priceCatalogDetail = null;
                currentLine.HasCustomPrice = hasCustomPrice;
                if (hasCustomPrice || documentHeader.IsCancelingAnotherDocument)
                {
                    if (unitPrice <= 0 && documentHeader.DocumentType != null && documentHeader.DocumentType.AllowItemZeroPrices == false)
                    {
                        unitPrice = 1;
                    }
                    price = unitPrice;
                    vatIncluded = !(documentHeader.DocumentType.IsForWholesale || documentHeader.Division == eDivision.Purchase);
                }
                else
                {
                    priceCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(databaseLayer, documentHeader.EffectivePriceCatalogPolicy(), item.Oid, item.Code, barcode?.Oid ?? Guid.Empty);
                    if (priceCatalogDetail == null && documentHeader.DocumentType.UsesPrices)
                    {
                        throw new Exception(string.Format(ITS.WRM.SFA.Resources.ResourcesRest.SelectedPriceCatalogPolicyNotContainsAllItems, currentLine.Item.Name));
                    }
                    if (priceCatalogDetail == null && documentHeader.DocumentType.AllowItemZeroPrices)
                    {
                        throw new Exception(string.Format(ITS.WRM.SFA.Resources.ResourcesRest.SelectedPriceCatalogPolicyNotContainsAllItems, currentLine.Item.Name));
                    }

                    price = priceCatalogDetail?.Value ?? 0m;
                    vatIncluded = priceCatalogDetail?.VATIncluded ?? true;
                    if (priceCatalogDetail != null && priceCatalogDetail.Discount > 0)
                    {
                        DocumentDetailDiscount existsingDiscount = currentLine.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.PRICE_CATALOG).FirstOrDefault();
                        if (existsingDiscount != null)
                        {
                            currentLine.DocumentDetailDiscounts.Remove(existsingDiscount);
                            App.DbLayer.DeleteObj<DocumentDetailDiscount>(existsingDiscount);
                        }
                        currentLine.DocumentDetailDiscounts.Add(PriceCatalogHelper.CreatePriceCatalogDetailDiscount(currentLine, priceCatalogDetail.Discount, databaseLayer));
                    }
                }

                if (is_linked_line && currentLine.UnitPrice < 0)
                {
                    price = .0m;
                }

                if (documentHeader.DocumentType.AllowItemZeroPrices == false && price == 0)
                {
                    throw new Exception(ITS.WRM.SFA.Resources.ResourcesRest.ItemsWithZeroPricesAreNotAllowed);
                }

                //Compute Packing Qty Measurement Unit
                if (documentHeader.DocumentType != null && documentHeader.DocumentType.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.DEFAULT))
                {
                    currentLine.Qty = qty;
                    currentLine.PackingQuantity = qty;
                    MeasurementUnit mu = currentLine.Barcode.MeasurementUnit(databaseLayer, documentHeader.Owner);
                    currentLine.PackingMeasurementUnit = mu;
                    currentLine.MeasurementUnit = mu;
                    currentLine.PackingMeasurementUnitOid = mu?.Oid ?? Guid.Empty;
                    currentLine.MeasurementUnitOid = mu?.Oid ?? Guid.Empty;
                    currentLine.PackingMeasurementUnitRelationFactor = 1;
                    currentLine.CustomMeasurementUnit = mu?.Description ?? "";

                }
                else if (documentHeader.DocumentType != null && documentHeader.DocumentType.MeasurementUnitMode == (eDocumentTypeMeasurementUnit.PACKING))
                {
                    ItemBarcode taxCodeItemBarcode = ItemHelper.GetTaxCodeBarcode(databaseLayer, currentLine.Item, documentHeader.Owner, OwnerApplicationSettings, currentLine.Barcode);
                    if (taxCodeItemBarcode == null)
                    {
                        throw new Exception(string.Format(ITS.WRM.SFA.Resources.ResourcesRest.PleaseDefineTaxCodeBarcodeForItem, currentLine.Item.Name));
                    }
                    if (taxCodeItemBarcode.MeasurementUnit == null)
                    {
                        taxCodeItemBarcode.MeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == taxCodeItemBarcode.MeasurementUnitOid).FirstOrDefault();
                    }
                    if (currentLine.Item.PackingMeasurementUnit == null && currentLine.Item.PackingMeasurementUnitOid != null && currentLine.Item.PackingMeasurementUnitOid != Guid.Empty)
                    {
                        currentLine.Item.PackingMeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == currentLine.Item.PackingMeasurementUnitOid).FirstOrDefault();
                    }
                    if (currentLine.Item.PackingQty <= 0 || (currentLine.Item.PackingMeasurementUnitOid == null || currentLine.Item.PackingMeasurementUnitOid == Guid.Empty)
                                                                           || currentLine.Item.PackingMeasurementUnitOid == taxCodeItemBarcode.MeasurementUnitOid)
                    {
                        currentLine.PackingMeasurementUnitRelationFactor = 1;
                        currentLine.PackingMeasurementUnit = taxCodeItemBarcode.MeasurementUnit;
                    }
                    else
                    {
                        currentLine.PackingMeasurementUnitRelationFactor = currentLine.Item.PackingQty;
                    }

                    currentLine.PackingQuantity = ItemHelper.GetPackingQuantity(qty, currentLine.PackingMeasurementUnitRelationFactor);
                    currentLine.PackingMeasurementUnit = currentLine.Item.PackingMeasurementUnit;
                    currentLine.PackingMeasurementUnitOid = currentLine.Item.PackingMeasurementUnitOid;

                    currentLine.Qty = qty;
                    currentLine.MeasurementUnit = taxCodeItemBarcode.MeasurementUnit;
                    currentLine.MeasurementUnitOid = taxCodeItemBarcode.MeasurementUnit?.Oid ?? Guid.Empty;
                    currentLine.CustomMeasurementUnit = currentLine.PackingMeasurementUnit != null ? currentLine.PackingMeasurementUnit.Description : "";
                }
                //currentLine.PackingMeasurementUnitOid = currentLine.PackingMeasurementUnitOid;
                //currentLine.MeasurementUnitOid = currentLine.MeasurementUnitOid;

                bool priceHasChanged = oldDocumentLine != null && oldDocumentLine.CustomUnitPrice != price;
                currentLine.CustomUnitPrice = currentLine.PriceListUnitPrice = price;
                if (hasCustomPrice || oldDocumentLine == null || priceHasChanged)
                {
                    if (vatIncluded)
                    {
                        currentLine.PriceListUnitPriceWithoutVAT = price / (1 + oldVatFactor.Factor);
                        currentLine.PriceListUnitPriceWithVAT = (price / (1 + oldVatFactor.Factor)) * (1 + currentLine.VatFactor);
                    }
                    else
                    {
                        currentLine.PriceListUnitPriceWithoutVAT = price;
                        currentLine.PriceListUnitPriceWithVAT = price * (1 + currentLine.VatFactor);
                    }
                }
                else
                {
                    currentLine.PriceListUnitPriceWithoutVAT = oldDocumentLine.PriceListUnitPriceWithoutVAT;
                    currentLine.PriceListUnitPriceWithVAT = currentLine.PriceListUnitPriceWithoutVAT * (1 + currentLine.VatFactor);
                }

                //CHECKING LIMITS
                if (currentLine.Qty > documentHeader.DocumentType.MaxDetailQty && documentHeader.DocumentType.MaxDetailQty > 0)
                {
                    throw new Exception(String.Format(ITS.WRM.SFA.Resources.ResourcesRest.MaxItemOrderQtyExceeded, currentLine.Qty));
                }

                if (price > documentHeader.DocumentType.MaxDetailValue && documentHeader.DocumentType.MaxDetailValue > 0)
                {
                    throw new Exception(String.Format(ITS.WRM.SFA.Resources.ResourcesRest.InvalidDetailValue, price));
                }

                if (Math.Abs(currentLine.Qty * price) > documentHeader.DocumentType.MaxDetailTotal && documentHeader.DocumentType.MaxDetailTotal > 0)
                {
                    throw new Exception(String.Format(ResourcesRest.InvalidDetailTotal, currentLine.Qty * price));
                }

                if (documentHeader.DocumentType.IsForWholesale || documentHeader.Division == eDivision.Purchase)
                {
                    //Round στην τιμή, με βάση
                    currentLine.PriceListUnitPrice = RoundDisplayValueDigits(currentLine.PriceListUnitPrice, OwnerApplicationSettings);
                    currentLine.PriceListUnitPriceWithoutVAT = RoundDisplayValueDigits(currentLine.PriceListUnitPriceWithoutVAT, OwnerApplicationSettings);
                    WholesaleDocumentDetail(ref currentLine, documentHeader, vatIncluded, OwnerApplicationSettings, databaseLayer);
                }
                else
                {
                    currentLine.PriceListUnitPrice = vatIncluded ? RoundDisplayValue(currentLine.PriceListUnitPrice, App.OwnerApplicationSettings) : RoundValue(currentLine.PriceListUnitPrice, App.OwnerApplicationSettings);
                    currentLine.PriceListUnitPriceWithVAT = RoundDisplayValue(currentLine.PriceListUnitPriceWithVAT, App.OwnerApplicationSettings);
                    RetailDocumentDetail(ref currentLine, documentHeader, App.OwnerApplicationSettings, vatIncluded, databaseLayer);
                }

                //Computes points of line if documenet type is Sales, otherwise set points to 0

                //if (documentHeader.Division == eDivision.Sales)
                //{

                //    decimal decimalPoints = itemHelper.GetPointsOfItem(databaseLayer, currentLine.Item, documentHeader.DocumentType, OwnerApplication/*ownerApplicationSettings*/) * Math.Floor(currentLine.Qty);
                //    int points = 0;
                //    if (decimalPoints >= int.MaxValue)
                //    {
                //        points = int.MaxValue;
                //    }
                //    else
                //    {
                //        points = (int)decimalPoints;
                //    }
                //    currentLine.Points = points;
                //}
                //else
                //{
                //    currentLine.Points = 0;
                //}

                //SOS Calculate Points
                // currentLine.Points = itemHelper.GetPointsOfItem(databaseLayer, currentLine.Item, documentHeader.DocumentType, OwnerApplication/*ownerApplicationSettings*/) * Math.Floor(currentLine.Qty);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                if (isNew)
                {
                    databaseLayer.DeleteDocumentDetail(currentLine);
                }
                throw new Exception(ex.Message + System.Environment.NewLine + currentLine.Item.Name);
            }
            RecalculateDocumentCosts(ref documentHeader, databaseLayer, false, false);
            return currentLine;
        }

        public static decimal RoundWheightValue(decimal value)
        {
            return Math.Round(value, 3, MidpointRounding.AwayFromZero);
        }

        public static decimal RoundValue(decimal value, OwnerApplicationSettings ownerApplicationSettings)
        {
            return Math.Round(value, (int)ownerApplicationSettings.ComputeDigits, MidpointRounding.AwayFromZero);
        }

        private static decimal RoundDisplayValueDigits(decimal value, OwnerApplicationSettings ownerApplicationSettings)
        {
            return Math.Round(value, (int)ownerApplicationSettings.DisplayValueDigits, MidpointRounding.AwayFromZero);
        }

        public static decimal RoundDisplayValue(decimal value, OwnerApplicationSettings ownerApplicationSettings)
        {
            return Math.Round(value, (int)ownerApplicationSettings.DisplayDigits, MidpointRounding.AwayFromZero);
        }

        private static void WholesaleDocumentDetail(ref DocumentDetail documentDetail, DocumentHeader header, bool vatIncluded, OwnerApplicationSettings ownerApplicationSettings, DatabaseLayer databaseLayer)
        {
            if (!header.DocumentType.IsForWholesale && header.Division != eDivision.Purchase)
            {
                throw new Exception("This code has been called incorrectly....");
            }

            documentDetail.UnitPrice = documentDetail.PriceListUnitPriceWithoutVAT;//documentDetail.UnitPrice = documentDetail.PriceListUnitPrice / (1 + (vatIncluded == true ? documentDetail.VatFactor : 0.0m));
            documentDetail.NetTotalBeforeDiscount = RoundDisplayValue(documentDetail.UnitPrice * documentDetail.Qty, ownerApplicationSettings);
            documentDetail.GrossTotalBeforeDiscount = RoundDisplayValue(documentDetail.NetTotalBeforeDiscount * (1 + documentDetail.VatFactor), ownerApplicationSettings);
            documentDetail.TotalVatAmountBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.NetTotalBeforeDiscount, ownerApplicationSettings);

            CalculateDetailTotalDiscount(ref documentDetail, header, ownerApplicationSettings, databaseLayer);
            documentDetail.GrossTotalBeforeDocumentDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalNonDocumentDiscount, ownerApplicationSettings);
            documentDetail.NetTotal = RoundDisplayValue(documentDetail.NetTotalBeforeDiscount - documentDetail.TotalDiscount, ownerApplicationSettings);
            documentDetail.TotalVatAmount = RoundDisplayValue(documentDetail.NetTotal * documentDetail.VatFactor, ownerApplicationSettings);
            documentDetail.GrossTotal = RoundDisplayValue(documentDetail.NetTotal + documentDetail.TotalVatAmount, ownerApplicationSettings);
            documentDetail.FinalUnitPrice = documentDetail.NetTotal / documentDetail.Qty;
            documentDetail.CustomUnitPrice = documentDetail.UnitPrice;
        }

        /// <summary>
        /// Calculates the TotalDiscount field of the detail using the DocumentDetailDiscounts list
        /// </summary>
        /// <param name="documentDetail"></param>
        private static void CalculateDetailTotalDiscount(ref DocumentDetail documentDetail, DocumentHeader header, OwnerApplicationSettings ownerApplicationSettings, DatabaseLayer dblayer)
        {
            bool isWholeSale = header.DocumentType.IsForWholesale;
            decimal totalDiscount = 0, originalAmount;
            decimal amountToApplyDiscount = originalAmount = isWholeSale ? documentDetail.NetTotalBeforeDiscount : documentDetail.GrossTotalBeforeDiscount;

            DocumentDetailDiscount overridesAllDiscount = documentDetail.DocumentDetailDiscounts.OrderBy(x => x.Priority).FirstOrDefault(x => x.DiscardsOtherDiscounts);
            if (overridesAllDiscount != null)
            {
                documentDetail.TotalDiscount = CalculateDiscountAmount(overridesAllDiscount, amountToApplyDiscount);
                if (isWholeSale)
                {
                    overridesAllDiscount.DiscountWithoutVAT = overridesAllDiscount.Value;
                    overridesAllDiscount.DiscountWithVAT = overridesAllDiscount.Value * (1 + documentDetail.VatFactor);
                }
                else
                {
                    overridesAllDiscount.DiscountWithVAT = overridesAllDiscount.Value;
                    overridesAllDiscount.DiscountWithoutVAT = overridesAllDiscount.Value / (1 + documentDetail.VatFactor);
                }
                return;
            }
            List<DocumentDetailDiscount> Discounts = documentDetail.DocumentDetailDiscounts;
            foreach (DocumentDetailDiscount discount in Discounts.OrderBy(x => x.Priority))
            {
                DocumentDetailDiscount existsingDiscount = documentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == discount.DiscountSource).FirstOrDefault();
                if (existsingDiscount != null)
                {
                    documentDetail.DocumentDetailDiscounts.Remove(existsingDiscount);
                    App.DbLayer.DeleteObj<DocumentDetailDiscount>(existsingDiscount);
                }
                if (discount.Percentage == 0)
                {
                    if (amountToApplyDiscount != 0)
                    {
                        discount.Percentage = discount.Value / amountToApplyDiscount;
                    }
                }
                else
                {

                    discount.Value = (discount.Percentage) * amountToApplyDiscount;
                }
                decimal discountAmount = RoundDisplayValue(CalculateDiscountAmount(discount, amountToApplyDiscount), ownerApplicationSettings);
                totalDiscount += discountAmount;
                amountToApplyDiscount = originalAmount - totalDiscount;
                if (isWholeSale)
                {
                    discount.DiscountWithoutVAT = RoundDisplayValue(discount.Value, App.OwnerApplicationSettings);
                    discount.DiscountWithVAT = RoundDisplayValue(discount.Value * (1 + documentDetail.VatFactor), App.OwnerApplicationSettings);
                }
                else
                {
                    discount.DiscountWithVAT = RoundDisplayValue(discount.Value, App.OwnerApplicationSettings);
                    discount.DiscountWithoutVAT = RoundDisplayValue(discount.Value / (1 + documentDetail.VatFactor), App.OwnerApplicationSettings);
                }
                documentDetail.DocumentDetailDiscounts.Add(discount);
                dblayer.InsertOrReplaceObj<DocumentDetailDiscount>(discount);
            }

            documentDetail.TotalDiscount = totalDiscount;
            if (isWholeSale)
            {
                documentDetail.TotalDiscountAmountWithoutVAT = RoundDisplayValue(documentDetail.TotalDiscount, App.OwnerApplicationSettings);
                documentDetail.TotalDiscountAmountWithVAT = RoundDisplayValue(documentDetail.TotalDiscount * (1 + documentDetail.VatFactor), App.OwnerApplicationSettings);
            }
            else
            {
                documentDetail.TotalDiscountAmountWithoutVAT = RoundDisplayValue(documentDetail.TotalDiscount / (1 + documentDetail.VatFactor), App.OwnerApplicationSettings);
                documentDetail.TotalDiscountAmountWithVAT = RoundDisplayValue(documentDetail.TotalDiscount, App.OwnerApplicationSettings);
            }

        }

        private static decimal CalculateDiscountAmount(DocumentDetailDiscount discount, decimal amountToApplyDiscount)
        {
            decimal discountValue = 0;

            if (discount.DiscountType == eDiscountType.PERCENTAGE)
            {
                discountValue = RoundDisplayValue(amountToApplyDiscount * discount.Percentage, App.OwnerApplicationSettings);
                discount.Value = discountValue;
            }
            else
            {
                discountValue = RoundDisplayValue(discount.Value, App.OwnerApplicationSettings);
            }

            return discountValue;
        }

        /*
        private Guid _LinkedLine;
        private double _UnitPriceAfterDiscount;             // Τελικη τιμή άνευ ΦΠΑ μετά έκπτωσης //Εδώ θα εφαρμοστεί η Δεύτερη Έκπτωση στην περίπτωση ΧΟΝΤΡΙΚΗΣ
        private double _PriceListUnitPrice;                 // Αρχική τιμή άνευ ΦΠΑ ή τιμή καρφωτή/χρήστη
        private double _UnitPrice;                          // PriceListUnitPrice μετά έκπτωσης πελάτη προ ΦΠΑ
        private double _Qty;                                // Ποσότητα
        private double _FirstDiscount;                      // Εκπτωση τιμοκαταλόγου ανά τεμάχιο
        private double _SecondDiscount;                     // 2η Εκπτωση (επιπλέον) ανά τεμάχιο
        private double _VatFactor;                          // Ποσοστό ΦΠΑ
        private double _VatAmount;                          // Ποσό ΦΠΑ ανά τεμάχιο
        private double _NetTotalAfterDiscount;              // Καθαρή αξία (μετά εκπτώσεων)
        private double _GrossTotal;                         // Συνολική αξία γραμμής (με ΦΠΑ)
        private double _FinalUnitPrice;                     // Τελική τιμή μονάδος με ΦΠΑ //Εδώ θα εφαρμοστεί η Δεύτερη Έκπτωση στην περίπτωση ΛΙΑΝΙΚΗΣ
        private double _TotalDiscount;                      // Συνολικό ποσό έκπτωσης
        private double _TotalVatAmount;                     // Συνολικό ποσό ΦΠΑ
        private double _NetTotal;                           // Καθαρή αξία (προ εκπτώσεων)
        *
        * Γνωστά
        * _FirstDiscount;
        * _SecondDiscount;
        * _VatFactor
        * _Qty
        * _PriceListUnitPrice (with or without VAT);
        * * */
        private static void RetailDocumentDetail(ref DocumentDetail documentDetail, DocumentHeader header, OwnerApplicationSettings ownerApplicationSettings, bool vatIncluded, DatabaseLayer databaseLayer)
        {
            if (header.DocumentType.IsForWholesale)
            {
                throw new Exception("This code has been called incorrectly....");
            }

            if (vatIncluded)
            {
                documentDetail.CustomUnitPrice = RoundValue(documentDetail.PriceListUnitPriceWithoutVAT * (1m + documentDetail.VatFactor), ownerApplicationSettings);
                documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPriceWithoutVAT, ownerApplicationSettings);
            }
            else
            {
                documentDetail.UnitPrice = RoundValue(documentDetail.PriceListUnitPriceWithoutVAT, ownerApplicationSettings);
                documentDetail.CustomUnitPrice = RoundDisplayValue(documentDetail.PriceListUnitPriceWithoutVAT, ownerApplicationSettings);
            }
            documentDetail.GrossTotalBeforeDiscount = RoundValue(documentDetail.CustomUnitPrice * documentDetail.Qty, ownerApplicationSettings);
            documentDetail.TotalVatAmountBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount * documentDetail.VatFactor / (1 + documentDetail.VatFactor), ownerApplicationSettings);
            documentDetail.NetTotalBeforeDiscount = RoundDisplayValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalVatAmountBeforeDiscount, ownerApplicationSettings);

            CalculateDetailTotalDiscount(ref documentDetail, header, ownerApplicationSettings, databaseLayer);

            documentDetail.GrossTotalBeforeDocumentDiscount = RoundValue(documentDetail.GrossTotalBeforeDiscount, ownerApplicationSettings) - RoundValue(documentDetail.TotalNonDocumentDiscount, ownerApplicationSettings);
            documentDetail.GrossTotal = RoundValue(documentDetail.GrossTotalBeforeDiscount - documentDetail.TotalDiscount, ownerApplicationSettings);
            documentDetail.TotalVatAmount = RoundDisplayValue(documentDetail.GrossTotal * documentDetail.VatFactor / (1 + documentDetail.VatFactor), ownerApplicationSettings);
            documentDetail.NetTotal = RoundDisplayValue(documentDetail.GrossTotal - documentDetail.TotalVatAmount, ownerApplicationSettings);
            documentDetail.FinalUnitPrice = RoundValue(documentDetail.NetTotal / documentDetail.Qty, ownerApplicationSettings);
            documentDetail.FinalUnitPrice = RoundValue(documentDetail.FinalUnitPrice, ownerApplicationSettings);
        }

        public static void RecalculateDocumentCosts(ref DocumentHeader documentHeader, DatabaseLayer databaseLayer, bool recompute_document_lines = true, bool ignoreOwnerSettings = false)
        {
            try
            {
                switch (documentHeader.Division)
                {
                    case eDivision.Purchase:
                        RecalculateDocumentCostsPurchase(ref documentHeader);
                        break;
                    case eDivision.Sales:
                        RecalculateDocumentCostsSales(ref documentHeader, databaseLayer, recompute_document_lines, ignoreOwnerSettings);
                        break;
                    case eDivision.Store:
                        RecalculateDocumentCostsSales(ref documentHeader, databaseLayer, false, false);
                        break;
                    case eDivision.Financial:
                        RecalculateDocumentCostsFinancial(ref documentHeader);
                        break;
                    case eDivision.Other:
                        RecalculateDocumentCostsSales(ref documentHeader, databaseLayer, false, false);
                        break;
                    default:
                        throw new InvalidOperationException("Unreachable code exception - DocumentHelper.RecalculateDocumentCosts()");
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Επαναυπολογίζει τα κόστη του Παραστατικού Αγοράς.
        /// </summary>
        /// <param name="documentHeader"></param>
        private static void RecalculateDocumentCostsPurchase(ref DocumentHeader documentHeader)
        {
            documentHeader.GrossTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotal);
            documentHeader.NetTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotal);
            documentHeader.TotalVatAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmount);
            documentHeader.GrossTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDiscount);
            documentHeader.NetTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotalBeforeDiscount);
            documentHeader.TotalVatAmountBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmountBeforeDiscount);
            documentHeader.GrossTotalBeforeDocumentDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDocumentDiscount);
            documentHeader.TotalDiscountAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalDiscount);
            documentHeader.TotalQty = documentHeader.DocumentDetails.Where(detail => !detail.IsCanceled).Sum(detail => detail.Qty);

            documentHeader.DocumentPoints = 0;
            documentHeader.TotalPoints = 0;
        }

        private static void RecalculateDocumentCostsFinancial(ref DocumentHeader documentHeader)
        {

            documentHeader.GrossTotal = documentHeader.DocumentPayments.Sum(documentPayment => documentPayment.Amount);
            documentHeader.NetTotal = documentHeader.GrossTotal;
            documentHeader.TotalVatAmount = 0;
            documentHeader.GrossTotalBeforeDiscount = documentHeader.GrossTotal;
            documentHeader.NetTotalBeforeDiscount = documentHeader.GrossTotal;
            documentHeader.TotalVatAmountBeforeDiscount = 0;
            documentHeader.GrossTotalBeforeDocumentDiscount = documentHeader.GrossTotal;
            documentHeader.TotalDiscountAmount = 0;
            documentHeader.TotalQty = 1;
            documentHeader.DocumentPoints = 0;
            documentHeader.TotalPoints = 0;

        }

        /// <summary>
        /// Επαναυπολογίζει τα κόστη της παραγγελίας. Ανάλογα με την τιμή της recompute_document_lines
        /// απλώς προσθέτει τα κόστη των γραμμών ή επαναυπολογίζει και τις γραμμές
        /// </summary>
        /// <param name="documentHeader"></param>
        /// <param name="recompute_document_lines"></param>
        /// <param name="ignoreOwnerSettings"></param>
        private static void RecalculateDocumentCostsSales(ref DocumentHeader documentHeader, DatabaseLayer databaseLayer, bool recompute_document_lines, bool ignoreOwnerSettings)
        {
            try
            {
                OwnerApplicationSettings ownerApplicationSettings = App.OwnerApplicationSettings;
                if (recompute_document_lines || ignoreOwnerSettings)
                {
                    foreach (DocumentDetail documentDetail in documentHeader.DocumentDetails)
                    {
                        DocumentDetail tempDocumentLine;
                        if (ownerApplicationSettings.RecomputePrices || ignoreOwnerSettings)
                        {
                            tempDocumentLine = ComputeDocumentLine(ref documentHeader, databaseLayer,
                                documentDetail.Item, documentDetail.Barcode, documentDetail.PackingQuantity, documentDetail.IsLinkedLine, -1,
                                false, documentDetail.CustomDescription, documentDetail.DocumentDetailDiscounts, true, documentDetail.CustomMeasurementUnit, documentDetail);
                        }
                        else
                        {
                            tempDocumentLine = ComputeDocumentLine(ref documentHeader, databaseLayer,
                                documentDetail.Item, documentDetail.Barcode, documentDetail.PackingQuantity, documentDetail.IsLinkedLine, documentDetail.CustomUnitPrice,
                                true, documentDetail.CustomDescription, documentDetail.DocumentDetailDiscounts, true, documentDetail.CustomMeasurementUnit,
                                documentDetail);
                        }
                    }
                }
                documentHeader.GrossTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDiscount);
                documentHeader.GrossTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotal);
                documentHeader.NetTotalBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotalBeforeDiscount);
                documentHeader.NetTotal = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.NetTotal);
                documentHeader.TotalVatAmountBeforeDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmountBeforeDiscount);
                documentHeader.TotalVatAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalVatAmount);
                documentHeader.TotalDiscountAmount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.TotalDiscount);
                documentHeader.GrossTotalBeforeDocumentDiscount = documentHeader.SumarisableDocumentDetails.Sum(docDetail => docDetail.GrossTotalBeforeDocumentDiscount);
                documentHeader.DocumentPoints = 0;


            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw ex;
            }
            documentHeader.TotalQty = documentHeader.DocumentDetails.Where(detail => !detail.IsCanceled).Sum(detail => detail.Qty);
        }

        public static void AddItem(ref DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseLayer databaseLayer, bool add_linked_lines = true, bool recalculateHeader = true)
        {
            if (documentDetail.LineNumber == 0)
            {
                documentDetail.LineNumber = GetDocumentNextDocumentDetailSortOrder(documentHeader);
            }
            documentHeader.DocumentDetails.Add(documentDetail);

            databaseLayer.InsertNewDocumentDetail(documentDetail);
            if (add_linked_lines && documentHeader.Division == eDivision.Sales)
            {
                AddLinkedItems(ref documentHeader, documentDetail, databaseLayer);
            }
            if (recalculateHeader)
            {
                RecalculateDocumentCosts(ref documentHeader, databaseLayer, false, false);
            }
        }

        public static int GetDocumentNextDocumentDetailSortOrder(DocumentHeader documentHeader)
        {
            return documentHeader.DocumentDetails.Count == 0 ? 1 : documentHeader.DocumentDetails.Max(documentDetail => documentDetail.LineNumber) + 1;
        }

        private static void AddLinkedItems(ref DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseLayer databaseLayer)
        {
            try
            {
                int offset = 1;
                EffectivePriceCatalogPolicy currentPriceCatalogPolicy = documentHeader.EffectivePriceCatalogPolicy();
                OwnerApplicationSettings ownerApplicationSettings = App.OwnerApplicationSettings;
                List<LinkedItem> linkedItems = databaseLayer.GetLinkedItemsCustom(documentDetail.ItemOid);
                documentDetail.Item.LinkedItems = linkedItems;
                if (documentDetail.Item.LinkedItems != null)
                {
                    foreach (LinkedItem linked_item in documentDetail.Item.LinkedItems)
                    {
                        if (linked_item.QtyFactor <= 0)
                        {
                            throw new Exception("InvalidQuantityFactorForLinkedItemOnItem");
                        }

                        PriceCatalogDetail tempPriceCatalogDetail = null;
                        if (tempPriceCatalogDetail == null && linked_item.SubItem != null)
                        {
                            tempPriceCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(databaseLayer, currentPriceCatalogPolicy, linked_item.SubItem.Oid, linked_item.SubItem.Code, Guid.Empty);
                        }

                        if (tempPriceCatalogDetail == null && linked_item.SubItem != null && linked_item.SubItem.DefaultBarcode != null)
                        {
                            string search_code_str = linked_item.SubItem.DefaultBarcode.Code;
                            if (ownerApplicationSettings.PadBarcodes)
                            {
                                search_code_str = search_code_str.PadLeft(ownerApplicationSettings.BarcodeLength, ownerApplicationSettings.BarcodePaddingCharacter[0]);
                            }
                            tempPriceCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(databaseLayer, currentPriceCatalogPolicy, linked_item.SubItem.Oid, linked_item.SubItem.Code, linked_item.SubItem.DefaultBarcodeOid);

                        }

                        if (tempPriceCatalogDetail == null && linked_item.SubItem.Code != null)
                        {
                            string search_code_str = linked_item.SubItem.Code;
                            if (ownerApplicationSettings.PadItemCodes)
                            {
                                search_code_str = search_code_str.PadLeft(ownerApplicationSettings.ItemCodeLength, ownerApplicationSettings.ItemCodePaddingCharacter[0]);
                            }
                            Barcode bc = databaseLayer.GetBarcodeByCode(search_code_str);
                            tempPriceCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(databaseLayer, currentPriceCatalogPolicy, linked_item.SubItem.Oid, linked_item.SubItem.Code, bc?.Oid ?? Guid.Empty);
                            if (tempPriceCatalogDetail == null)
                            {
                                search_code_str = linked_item.SubItem.Code;
                                if (ownerApplicationSettings.PadBarcodes)
                                {
                                    search_code_str = search_code_str.PadLeft(ownerApplicationSettings.BarcodeLength, ownerApplicationSettings.BarcodePaddingCharacter[0]);
                                    bc = databaseLayer.GetBarcodeByCode(search_code_str);
                                }
                                tempPriceCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(databaseLayer, currentPriceCatalogPolicy, linked_item.SubItem.Oid, linked_item.SubItem.Code, bc?.Oid ?? Guid.Empty);
                            }
                        }


                        if (tempPriceCatalogDetail == null)
                        {
                            #region Add LinkedItem with zero value
                            decimal pcDiscount = tempPriceCatalogDetail == null ? .0m : tempPriceCatalogDetail.Discount;
                            Barcode search_barcode = linked_item.SubItem.DefaultBarcode;
                            if (search_barcode == null)
                            {
                                string search_code_str = linked_item.SubItem.Code;
                                if (ownerApplicationSettings.PadBarcodes)
                                {
                                    search_code_str = search_code_str.PadLeft(ownerApplicationSettings.BarcodeLength, ownerApplicationSettings.BarcodePaddingCharacter[0]);
                                }
                                search_barcode = databaseLayer.GetBarcodeByCode(search_code_str);
                            }

                            if (search_barcode == null)
                            {
                                DeleteItem(ref documentHeader, documentDetail, databaseLayer);
                                throw new Exception("LinkedItemWithoutPriceHasBeenFound");
                            }
                            List<DocumentDetailDiscount> DiscountList = documentDetail.DocumentDetailDiscounts;
                            DocumentDetail oldDocumentDetail = documentDetail;
                            DocumentDetail tempDocumentDetail = ComputeDocumentLine(ref documentHeader, databaseLayer, linked_item.SubItem, search_barcode, documentDetail.Qty * (decimal)linked_item.QtyFactor,
                                                                                                                                                                    true, -1, false, linked_item.SubItem.Name, null);

                            tempDocumentDetail.LinkedLine = documentDetail.Oid;

                            if (tempDocumentDetail.UnitPrice < 0)
                            {
                                tempDocumentDetail.UnitPrice = 0;
                            }
                            tempDocumentDetail.LineNumber = documentDetail.LineNumber + offset;
                            offset++;
                            documentHeader.DocumentDetails.Add(tempDocumentDetail);
                            databaseLayer.InsertNewDocumentDetail(tempDocumentDetail);
                            return;
                            #endregion
                        }
                        //pass values 
                        Guid subItemOid = linked_item.SubItem.Oid;
                        var currentBarcode = linked_item.SubItem?.ItemBarcodes.Where(x => x.ItemOid.Equals(subItemOid))?.Select(x => x.BarcodeOid).FirstOrDefault() ?? Guid.Empty;
                        Guid subItemBarcode = currentBarcode;

                        //
                        DocumentDetail tempDocumentDetail2 = ComputeDocumentLine(ref documentHeader, databaseLayer, linked_item.SubItem, tempPriceCatalogDetail.Barcode, documentDetail.Qty * (decimal)linked_item.QtyFactor,
                                                                                                                                                                                                    true, -1, false, "", null);

                        tempDocumentDetail2.LinkedLine = documentDetail.Oid;
                        if (tempDocumentDetail2.UnitPrice < 0)
                        {
                            tempDocumentDetail2.UnitPrice = 0;
                        }

                        tempDocumentDetail2.LineNumber = documentDetail.LineNumber + offset;
                        offset++;
                        documentHeader.DocumentDetails.Add(tempDocumentDetail2);
                        databaseLayer.InsertNewDocumentDetail(tempDocumentDetail2);
                    }
                }

            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        public static void DeleteItem(ref DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseLayer databaseLayer)
        {
            DeleteLinkedItems(ref documentHeader, documentDetail, databaseLayer);
            DeleteDiscounts(ref documentDetail, databaseLayer);
            documentHeader.DocumentDetails.Remove(documentDetail);
            databaseLayer.DeleteDocumentDetail(documentDetail);
        }

        public static void DeleteLinkedItems(ref DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseLayer databaseLayer)
        {
            IEnumerable<DocumentDetail> linkedLines = documentHeader.DocumentDetails.Where(x => x.LinkedLine == documentDetail.Oid);
            DocumentDetail detailToDelete = documentHeader.DocumentDetails.Where(x => x.LinkedLine == documentDetail.Oid).FirstOrDefault();
            foreach (DocumentDetail dtl in linkedLines)
            {
                DeleteItem(ref documentHeader, dtl, databaseLayer);
            }
        }

        public static void DeleteDiscounts(ref DocumentDetail documentDetail, DatabaseLayer databaseLayer)
        {
            List<DocumentDetailDiscount> discounts = databaseLayer.GetDocumentDetailDiscounts(documentDetail.Oid);
            foreach (DocumentDetailDiscount disc in discounts)
            {
                if (documentDetail.DocumentDetailDiscounts != null)
                {
                    documentDetail.DocumentDetailDiscounts.Remove(disc);
                }
                databaseLayer.DeleteObj<DocumentDetailDiscount>(disc);
            }
        }

        public static void SetFinancialDocumentDetail(ref DocumentHeader documentHeader)
        {
            DocumentDetail documentDetail = null;
            documentHeader.DocumentDetails = App.DbLayer.LoadDocumentFromDatabase(documentHeader.Oid).DocumentDetails;
            if (documentHeader.DocumentDetails.Count == 0)
            {
                documentDetail = new DocumentDetail();
                documentDetail.DocumentHeader = documentHeader;
                documentDetail.DocumentHeaderOid = documentHeader.Oid;
                documentHeader.DocumentDetails.Add(documentDetail);
            }
            else if (documentHeader.DocumentDetails.Count == 1)
            {
                documentDetail = documentHeader.DocumentDetails.First();
            }
            else
            {
                throw new Exception("Document must have one and only one DocumentDetail");
            }

            decimal totalAmount = documentHeader.DocumentPayments.Sum(payment => payment.Amount);
            documentHeader.NetTotal = totalAmount;
            documentHeader.NetTotalBeforeDiscount = totalAmount;
            documentHeader.GrossTotal = totalAmount;
            documentHeader.GrossTotalBeforeDiscount = totalAmount;
            if (documentHeader.DocumentType.SpecialItem == null)
            {
                documentHeader.DocumentType.SpecialItem = App.DbLayer.GetById<SpecialItem>(documentHeader.DocumentType.SpecialItemOid);
            }
            documentDetail.SpecialItem = documentHeader.DocumentType.SpecialItem;
            documentDetail.SpecialItemOid = documentHeader.DocumentType.SpecialItemOid;
            documentDetail.Qty = 1;
            documentDetail.NetTotal = documentHeader.NetTotal;
            documentDetail.NetTotalBeforeDiscount = documentHeader.NetTotalBeforeDiscount;
            documentDetail.GrossTotal = documentHeader.GrossTotal;
            documentDetail.GrossTotalBeforeDiscount = documentHeader.GrossTotalBeforeDocumentDiscount;

            RecalculateDocumentCostsFinancial(ref documentHeader);
            App.DbLayer.InsertOrReplaceObj<DocumentDetail>(documentDetail);
            App.DbLayer.UpdateDocumentHeader(documentHeader);
            documentHeader.DocumentDetails = new List<DocumentDetail>() { documentDetail };
        }

        public static void ReplaceItem(ref DocumentHeader documentHeader, DocumentDetail old_value, DocumentDetail new_value, DatabaseLayer databaseLayer)
        {
            new_value.LineNumber = old_value.LineNumber;
            DeleteItem(ref documentHeader, old_value, databaseLayer);
            AddItem(ref documentHeader, new_value, databaseLayer);
        }

        public static DocumentDetail CreateNewDocumentDetail(ref DocumentHeader documentHeader, Item item, Barcode barcode, Guid barcodeOid, decimal qty, DatabaseLayer dbLayer)
        {
            if (barcode == null)
            {
                barcode = dbLayer.GetBarcodeById(barcodeOid);
            }
            if (documentHeader.DocumentType == null)
            {
                Guid docTypeOid = documentHeader.DocumentTypeOid;
                documentHeader.DocumentType = App.DocumentTypes.Where(x => x.Oid == docTypeOid).FirstOrDefault();
            }
            if (!documentHeader.DocumentType.ManualLinkedLineInsertion)
            {
                List<LinkedItem> LinkedItems = dbLayer.GetLinkedItemsCustom(item.Oid);
                foreach (var CurrentItem in LinkedItems)
                {
                    CurrentItem.Item = item;
                    CurrentItem.ItemOid = item.Oid;
                }
                item.LinkedItems = LinkedItems;
            }
            item.DefaultBarcode = barcode;
            item.DefaultBarcodeOid = barcode.Oid;
            return ComputeDocumentLine(documentHeader: ref documentHeader, databaseLayer: dbLayer, item: item, barcode: barcode, qty: qty, is_linked_line: false, unitPrice: -1, hasCustomPrice: false, customDescription: item.Name, discounts: null);
        }

        public static string SignDocumentIfNecessary(Guid documentOid)
        {

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                DocumentHeader documentHeader = uow.GetObjectByKey<DocumentHeader>(documentOid);

                if (documentHeader.DocumentType.TakesDigitalSignature
                && documentHeader.DocumentNumber > 0
                && String.IsNullOrWhiteSpace(documentHeader.Signature)
               )
                {
                    try
                    {
                        StoreControllerSettings settings = uow.GetObjectByKey<StoreControllerSettings>(StoreControllerSettingsOid);
                        List<POSDevice> posDevices = settings.StoreControllerTerminalDeviceAssociations.
                            Where(x =>
                                    x.DocumentSeries.Any(y => y.DocumentSeries.Oid == documentHeader.DocumentSeries.Oid)
                                 && x.TerminalDevice is POSDevice
                                 && (x.TerminalDevice as POSDevice).DeviceSettings.DeviceType == DeviceType.DiSign
                            ).Select(x => x.TerminalDevice).Cast<POSDevice>().ToList();
                        string signature = DocumentHelper.SignDocument(documentHeader, user, documentHeader.Owner, String.Empty/*MvcApplication.OLAPConnectionString*/, posDevices);
                        if (string.IsNullOrWhiteSpace(signature))
                        {
                            return Resources.CannotRetreiveSignature;
                        }
                        documentHeader.Signature = signature;
                        documentHeader.Save();
                        XpoHelper.CommitTransaction(uow);
                    }
                    catch (Exception exception)
                    {
                        return exception.GetFullMessage();
                    }
                }
            }
            return String.Empty;
        }

        public static bool DocTypeSupportsCustomer(DocumentHeader document, Customer customer)
        {
            return true;
        }

        public static async Task<string> PrepareSaveDocument(DocumentHeader document, string txtDeliveryAddress, bool getLocation = false)
        {
            string errorMessage = string.Empty;
            if (document.IsSynchronized)
            {
                errorMessage = ResourcesRest.DocumentAlreadySend;
                return errorMessage;
            }
            if (document.TotalQty <= 0)
            {
                errorMessage = ResourcesRest.InsertDocumentDetails;
                return errorMessage;
            }
            if (document.BillingAddressOid == null || document.BillingAddressOid == Guid.Empty)
            {
                errorMessage = ResourcesRest.AddressNotFound;
                return errorMessage;
            }
            if (document.BillingAddress == null)
            {
                document.BillingAddress = App.DbLayer.GetById<Address>(document.BillingAddressOid);
                if (document.BillingAddress == null)
                {
                    errorMessage = ResourcesRest.AddressNotFound;
                    return errorMessage;
                }
            }
            if (string.IsNullOrEmpty(document.DeliveryAddress))
            {
                document.DeliveryAddress = txtDeliveryAddress;
            }
            if (string.IsNullOrEmpty(document.DeliveryAddress))
            {
                document.DeliveryAddress = document.BillingAddress?.City + " " + document.BillingAddress?.Street + " " + document.BillingAddress?.PostCode;
            }
            if (string.IsNullOrEmpty(document.DeliveryAddress))
            {
                errorMessage = ResourcesRest.DeliveryAddress;
                return errorMessage;
            }
            if (document.DocumentType == null)
            {
                document.DocumentType = App.DbLayer.GetDocumentTypeById(document.DocumentTypeOid);
            }
            if (document.DocumentType.UsesPaymentMethods && document.GrossTotal > 0 && document.StatusOid == App.SFASettings.DocumentStatusToSendOid)
            {
                if (document.DocumentPayments == null || document.DocumentPayments.Count == 0)
                {
                    errorMessage = ResourcesRest.MissingDocumentPayment;
                    return errorMessage;
                }
                decimal amount = document.GrossTotal - document.DocumentPayments.Sum(x => x.Amount);
                if (amount > 0)
                {
                    errorMessage = ResourcesRest.MissingDocumentPayment + " " + amount;
                    return errorMessage;
                }
            }
            if (document.Division == eDivision.Financial)
            {
                SetFinancialDocumentDetail(ref document);
            }

            if (getLocation)
            {
                AddressLocation loc = await SetDocumentLocation(document);
            }

            return errorMessage;
        }

        private static async Task<AddressLocation> SetDocumentLocation(DocumentHeader document)
        {
            AddressLocation loc = null;
            try
            {
                loc = await LocationManager.GetCurrentlocation();
                if (loc != null)
                {
                    document.Longitude = loc.Longitude;
                    document.Latitude = loc.Latitude;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            return loc;
        }

        public static async Task<string> PrepareSendDocument(DocumentHeader document, string txtDeliveryAddress, bool getLocation = false)
        {
            string errorMessage = string.Empty;
            try
            {
                errorMessage = await PrepareSaveDocument(document, txtDeliveryAddress, getLocation);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (document.StatusOid != App.SFASettings.DocumentStatusToSendOid)
                    {
                        DocumentStatus docStatus = App.DbLayer.GetById<DocumentStatus>(App.SFASettings.DocumentStatusToSendOid);
                        errorMessage = ResourcesRest.ChangeDocumentStatus + " " + docStatus?.Description;
                        return errorMessage;
                    }
                    if (document.DocumentType == null)
                    {
                        document.DocumentType = App.DbLayer.GetDocumentTypeById(document.DocumentTypeOid);
                    }
                    if (document.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS && document.DocumentType.QuantityFactor > 0)
                    {
                        List<DocumentDetail> outOfStockItems = ItemHelper.CheckStockBeforeSendDocument(document);
                        if (outOfStockItems != null && outOfStockItems.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(ResourcesRest.DocumentHasItemsOutOfStock);
                            sb.AppendLine();
                            foreach (DocumentDetail dtl in outOfStockItems)
                            {
                                Item item = ItemHelper.GetItemFromDetail(dtl);
                                if (item != null)
                                {
                                    sb.Append(item.Code + ", ");
                                }
                            }
                            errorMessage = sb.ToString();
                            return errorMessage;
                        }
                    }
                }
                else
                {
                    return errorMessage;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                errorMessage = ex.Message;
            }
            return errorMessage;
        }


    }
}
