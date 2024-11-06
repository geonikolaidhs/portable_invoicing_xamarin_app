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
    [CreateOrUpdaterOrder(Order = 150, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class Reason: LookUp2Fields
    {
        [ManyToOne(CascadeOperations = CascadeOperation.All), JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public ReasonCategory Category { get; set; }
        [ForeignKey(typeof(ReasonCategory))]
        [JsonProperty("Category.Oid")]
        [Column("Category")]
        public Guid CategoryOid { get; set; }
    }
}
