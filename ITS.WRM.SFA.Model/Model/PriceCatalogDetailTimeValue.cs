using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 665, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogDetailTimeValue : BasicObj
    {

        public long TimeValueValidUntil { get; set; }

        public long TimeValueValidFrom { get; set; }

        public decimal TimeValue { get; set; }


        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public PriceCatalogDetail PriceCatalogDetail { get; set; }

        [Indexed]
        [ForeignKey(typeof(PriceCatalogDetail))]
        [Column("PriceCatalogDetail")]
        [JsonProperty(PropertyName = "PriceCatalogDetail.Oid")]
        public Guid PriceCatalogDetailOid { get; set; }

    }
}
