using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class EffectivePriceCatalogPolicyDetail
    {
        public Guid PriceCatalogOid { get; set; }
        public int Sort { get; set; }
        public PriceCatalogSearchMethod PriceCatalogSearchMethod { get; set; }
        public bool IsDefault { get; set; }

        public EffectivePriceCatalogPolicyDetail(PriceCatalogPolicyDetail priceCatalogPolicyDetail)
        {
            this.PriceCatalogOid = priceCatalogPolicyDetail.PriceCatalogOid;
            this.Sort = priceCatalogPolicyDetail.Sort;
            this.PriceCatalogSearchMethod = priceCatalogPolicyDetail.PriceCatalogSearchMethod;
            this.IsDefault = priceCatalogPolicyDetail.IsDefault;
        }

        public EffectivePriceCatalogPolicyDetail(PriceCatalogPolicyDetail priceCatalogPolicyDetail, int sortOffset) : this(priceCatalogPolicyDetail)
        {
            this.Sort += sortOffset;
        }
    }
}
