using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 60, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class StorePriceCatalogPolicy:BaseObj
    {
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public Store Store { get; set; }

        [Indexed]
        [ForeignKey(typeof(Store))]
        [Column("Store")]
        [JsonProperty(PropertyName = "Store.Oid")]
        public Guid StoreOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public PriceCatalogPolicy PriceCatalogPolicy { get; set; }

        [ForeignKey(typeof(PriceCatalogPolicy))]
        [Column("PriceCatalogPolicy")]
        [JsonProperty(PropertyName = "PriceCatalogPolicy.Oid")]
        public Guid PriceCatalogPolicyOid { get; set; }

        public bool IsDefault { get; set; }
        
    }
}
