using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 52, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogPolicyDetail : BasicObj
    {
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public PriceCatalog PriceCatalog { get; set; }
        
        [Indexed]
        [ForeignKey(typeof(PriceCatalog))]
        [Column("PriceCatalog")]
        [JsonProperty(PropertyName = "PriceCatalog.Oid")]
        public Guid PriceCatalogOid { get; set; }

        public int Sort { get; set; }

        public PriceCatalogSearchMethod PriceCatalogSearchMethod { get; set; }

        public bool IsDefault { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public PriceCatalogPolicy PriceCatalogPolicy { get; set; }

        [Indexed]
        [ForeignKey(typeof(PriceCatalogPolicy))]
        [Column("PriceCatalogPolicy")]
        [JsonProperty(PropertyName = "PriceCatalogPolicy.Oid")]
        public Guid PriceCatalogPolicyOid { get; set; }
    }
}
