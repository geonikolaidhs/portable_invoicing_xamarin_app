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
    [CreateOrUpdaterOrder(Order = 530, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PaymentMethodField: CustomField
    {
        [ForeignKey(typeof(PaymentMethod))]
        [Column("PaymentMethod")]
        [JsonProperty(PropertyName = "PaymentMethod.Oid")]
        Guid PaymentMethodOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public PaymentMethod PaymentMethod { get; set; }
        
    }
}
