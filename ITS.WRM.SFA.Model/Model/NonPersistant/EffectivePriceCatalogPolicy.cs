using ITS.WRM.SFA.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class EffectivePriceCatalogPolicy
    {
        public List<EffectivePriceCatalogPolicyDetail> PriceCatalogPolicyDetails { get; set; }

        public Guid Owner { get; set; }

        public EffectivePriceCatalogPolicy()
        {
            this.PriceCatalogPolicyDetails = new List<EffectivePriceCatalogPolicyDetail>();
        }

        public EffectivePriceCatalogPolicy(Store store, Customer customer = null)
        {
            PriceCatalogPolicy CustomerPriceCatalogPolicy = customer == null ? null : DependencyService.Get<ICrossPlatformMethods>().GetAllPriceCatalogPolicies().Where(x => x.Oid == customer.PriceCatalogPolicy).FirstOrDefault();
            GenerateEffectivePriceCatalogPolicy(store, CustomerPriceCatalogPolicy);
        }

        public EffectivePriceCatalogPolicy(Store store, PriceCatalogPolicy priceCatalogPolicy)
        {
            GenerateEffectivePriceCatalogPolicy(store, priceCatalogPolicy);
        }

        private void GenerateEffectivePriceCatalogPolicy(Store store, PriceCatalogPolicy priceCatalogPolicy)
        {
            try
            {
                this.PriceCatalogPolicyDetails = new List<EffectivePriceCatalogPolicyDetail>();
                if (priceCatalogPolicy != null)
                {
                    List<PriceCatalogPolicyDetail> policydetails = DependencyService.Get<ICrossPlatformMethods>().GetAllPriceCatalogPolicyDetails().Where(x => x.PriceCatalogPolicyOid == priceCatalogPolicy.Oid).ToList();
                    if (policydetails.Count > 0)
                    {
                        List<StorePriceList> storePriceList = DependencyService.Get<ICrossPlatformMethods>().GetStorePriceLists();
                        List<Guid> storePriceCatalogGuids = storePriceList.Select(x => x.PriceCatalogOid).ToList();
                        this.PriceCatalogPolicyDetails = policydetails.Where(x => storePriceCatalogGuids.Contains(x.PriceCatalogOid)).Select(z => new EffectivePriceCatalogPolicyDetail(z)).ToList();
                    }
                }
                int sortOffset = this.PriceCatalogPolicyDetails.Count == 0 ? 0 : this.PriceCatalogPolicyDetails.Max(x => x.Sort) + 1;
                if (priceCatalogPolicy == null && store != null)
                {
                    List<PriceCatalogPolicyDetail> storePolicydetails = DependencyService.Get<ICrossPlatformMethods>().GetAllPriceCatalogPolicyDetails().Where(x => x.PriceCatalogPolicyOid == store.DefaultPriceCatalogPolicyOid).ToList();
                    List<EffectivePriceCatalogPolicyDetail> effectiveStorePricePolicyDetails = storePolicydetails.Select(x => new EffectivePriceCatalogPolicyDetail(x, sortOffset)).ToList();
                    this.PriceCatalogPolicyDetails.AddRange(effectiveStorePricePolicyDetails);
                }
                if (store != null)
                {
                    this.Owner = store.OwnerOid;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EffectivePriceCatalogPolicy(PriceCatalogPolicy priceCatalogPolicy)
        {
            this.PriceCatalogPolicyDetails = priceCatalogPolicy.PriceCatalogPolicyDetails
                                                               .Select(policyDetail => new EffectivePriceCatalogPolicyDetail(policyDetail)).ToList();
        }

        public bool HasPolicyDetails()
        {
            return this.PriceCatalogPolicyDetails.Count > 0;
        }

        public List<EffectivePriceCatalogPolicyDetail> OrderedPriceCatalogDetail
        {
            get
            {
                return this.PriceCatalogPolicyDetails.OrderBy(policyDetail => policyDetail.Sort).ToList();
            }
        }
    }
}
