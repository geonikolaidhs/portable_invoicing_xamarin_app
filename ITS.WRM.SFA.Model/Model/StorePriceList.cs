using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
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
    [CreateOrUpdaterOrder(Order = 580, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class StorePriceList : BasicObj
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
        public PriceCatalog PriceList { get; set; }

        [ForeignKey(typeof(PriceCatalog))]
        [Column("PriceList")]
        [JsonProperty(PropertyName = "PriceList.Oid")]
        public Guid PriceCatalogOid { get; set; }


    }
}
