using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 90, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class VatFactor : LookUp2Fields
    {
        public decimal Factor { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public VatLevel VatLevel { get; set; }


        [ForeignKey(typeof(VatLevel))]
        [JsonProperty("VatLevel.Oid")]
        [Column("VatLevel")]
        public Guid VatLevelOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public VatCategory VatCategory { get; set; }

        [ForeignKey(typeof(VatCategory))]
        [JsonProperty("VatCategory.Oid")]
        [Column("VatCategory")]
        public Guid VatCategoryOid { get; set; }

        [JsonIgnore]
        public decimal Value
        {
            get
            {
                return this.Factor * 100m;
            }
        }
    }
}
