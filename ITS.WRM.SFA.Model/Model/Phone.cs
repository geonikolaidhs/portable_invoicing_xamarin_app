using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;

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
    [CreateOrUpdaterOrder(Order = 650, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Phone : BasicObj
    {

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public Address Address { get; set; }

        [ForeignKey(typeof(Address))]
        [JsonProperty("Address.Oid")]
        [Column("Address")]
        public Guid AddressOid { get; set; }

        public string Number { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [JsonProperty("Owner.Oid")]
        [Column("Owner")]
        [ForeignKey(typeof(CompanyNew))]
        public Guid OwnerOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public PhoneType PhoneType { get; set; }
        [ForeignKey(typeof(PhoneType))]
        [JsonProperty("PhoneType.Oid")]
        [Column("PhoneType")]
        public Guid PhoneTypeOid { get; set; }



        public string Description
        {
            get
            {
                return (this.PhoneType == null ? "" : this.PhoneType.Description + ":") + this.Number;
            }
        }
    }
}
