using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public static class PriceCatalogHelper
    {
        public static PriceCatalogDetail GetPriceCatalogDetailFromPolicy(DatabaseLayer databaseLayer, EffectivePriceCatalogPolicy effectivePriceCatalogPolicy, Guid itemOid, string itemCode,
                    Guid barcodeOid, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE, List<PriceSearchTraceStep> traces = null)
        {
            try
            {
                PriceCatalogDetail priceCatalogDetail = null;
                //TODO explicitly define what is the value for the case when item has no price at all for given policy
                if (effectivePriceCatalogPolicy != null && effectivePriceCatalogPolicy.HasPolicyDetails())
                {
                    foreach (EffectivePriceCatalogPolicyDetail effectivePriceCatalogPolicyDetail in effectivePriceCatalogPolicy.PriceCatalogPolicyDetails.OrderBy(policyDetail => policyDetail.Sort))
                    {
                        PriceCatalog priceCatalog = App.PriceCatalogs.Where(x => x.Oid == effectivePriceCatalogPolicyDetail.PriceCatalogOid).FirstOrDefault();
                        priceCatalogDetail = ItemHelper.GetPriceCatalogDetail(databaseLayer, itemOid, priceCatalog, barcodeOid, itemCode, priceCatalogSearchMethod, traces);
                        if (priceCatalogDetail != null)
                        {
                            return priceCatalogDetail;
                        }
                    }
                }
                return priceCatalogDetail;//TODO explicitly define what is the value for the case when item has no price at all for given policy
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        public static PriceCatalogPolicy GetPriceCatalogPolicy(DatabaseLayer databaseLayer, Store store, Customer customer)
        {

            PriceCatalogPolicy priceCatalogPolicy = null;
            List<StorePriceList> storePriceList = App.StorePriceList;
            IEnumerable<Guid> storePriceCatalogGuids = storePriceList.Select(x => x.PriceCatalogOid).ToList();

            PriceCatalogPolicy customerPriceCatalogPolicy = App.PriceCatalogPolicies.Where(x => x.Oid == customer.PriceCatalogPolicy).FirstOrDefault();
            PriceCatalogPolicy storeDefaultPolicy = App.PriceCatalogPolicies.Where(x => x.Oid == store.DefaultPriceCatalogPolicyOid).FirstOrDefault();
            if (storeDefaultPolicy == null)
            {
                throw new Exception(Resources.ResourcesRest.DefaultPolicyNotFound);
            }
            if (customerPriceCatalogPolicy == null)
            {
                priceCatalogPolicy = storeDefaultPolicy;
            }
            else
            {
                List<PriceCatalogPolicyDetail> dtls = App.PriceCatalogPolicyDetails.Where(x => x.PriceCatalogPolicyOid == customerPriceCatalogPolicy.Oid)?.ToList() ?? new List<PriceCatalogPolicyDetail>();

                if (customer != null && customer.PriceCatalogPolicy != null && dtls.Count > 0)
                {
                    List<StorePriceCatalogPolicy> storePolicies = databaseLayer.GetStorePriceCatalogPolicies(store.Oid);
                    if (storePolicies != null)
                    {
                        store.StorePriceCatalogPolicies = storePolicies;
                        var CurentStorePriceCatalog = storePolicies.Where(x => x.PriceCatalogPolicyOid == customer.PriceCatalogPolicy).FirstOrDefault();
                        if (CurentStorePriceCatalog != null)
                        {
                            priceCatalogPolicy = customerPriceCatalogPolicy;
                        }
                        else
                        {
                            priceCatalogPolicy = storeDefaultPolicy;
                        }
                    }
                }
                else
                {
                    priceCatalogPolicy = storeDefaultPolicy;
                }
            }

            return priceCatalogPolicy;
        }

        public static DocumentDetailDiscount CreatePriceCatalogDetailDiscount(DocumentDetail detail, decimal priceCatalogDiscount, DatabaseLayer databaseLayer)
        {
            DocumentDetailDiscount pcDiscount = databaseLayer.GetDocumentDetailPriceCatalogDiscountBy(detail.Oid) ?? new DocumentDetailDiscount();
            pcDiscount.Percentage = priceCatalogDiscount;
            pcDiscount.Priority = -1;
            pcDiscount.DiscountSource = eDiscountSource.PRICE_CATALOG;
            pcDiscount.DiscountType = eDiscountType.PERCENTAGE;
            pcDiscount.DocumentDetailOid = detail.Oid;
            pcDiscount.DocumentDetail = detail;
            databaseLayer.InsertOrReplaceObj<DocumentDetailDiscount>(pcDiscount);
            return pcDiscount;
        }

    }
}
