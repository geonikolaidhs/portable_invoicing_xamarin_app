using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 52, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Store : BasicObj
    {
        public string Name { get; set; }
        public Guid? CentralOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [ForeignKey(typeof(CompanyNew))]
        [Column("Owner")]
        [JsonProperty(PropertyName = "Owner.Oid")]
        public Guid OwnerOid { get; set; }


        public Guid ImageOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public Address Address { get; set; }

        [ForeignKey(typeof(Address))]
        [Column("Address")]
        [JsonProperty(PropertyName = "Address.Oid")]
        public Guid AddressOid { get; set; }


        public bool IsCentralStore { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public PriceCatalogPolicy DefaultPriceCatalogPolicy { get; set; }

        [ForeignKey(typeof(PriceCatalogPolicy))]
        [Column("DefaultPriceCatalogPolicy")]
        [JsonProperty(PropertyName = "DefaultPriceCatalogPolicy.Oid")]
        public Guid DefaultPriceCatalogPolicyOid { get; set; }

        public string Code { get; set; }

        public Guid ReferenceCompanyOid { get; set; }


        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<StorePriceList> StorePriceLists { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<StorePriceCatalogPolicy> StorePriceCatalogPolicies { get; set; }
    }
}
