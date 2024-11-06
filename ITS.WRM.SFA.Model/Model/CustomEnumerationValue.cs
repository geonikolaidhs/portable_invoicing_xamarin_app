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
    [CreateOrUpdaterOrder(Order = 194, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class CustomEnumerationValue : BaseObj
    {

        [Column("CustomEnumerationDefinition")]
        [ForeignKey(typeof(CustomEnumerationDefinition))]
        [JsonProperty("CustomEnumerationDefinition.Oid")]
        [JsonIgnore]
        public Guid CustomEnumerationDefinitionOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public CustomEnumerationDefinition CustomEnumerationDefinition { get; set; }

        public string Description { get; set; }
        public int Ordering { get; set; }
    }
}
