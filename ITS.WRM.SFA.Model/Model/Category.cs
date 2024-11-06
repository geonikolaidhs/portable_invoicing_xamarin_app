
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Interface;
using SQLite;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;

namespace ITS.WRM.SFA.Model.Model
{

    public class Category : LookupField, IOwner
    {

        public new string Description { get; set; }


        public string Code { get; set; }

        public string ReferenceCode { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [ForeignKey(typeof(CompanyNew))]
        [Column("Owner")]
        [JsonProperty(PropertyName = "Owner.Oid")]
        public Guid OwnerOid { get; set; }

        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }
    }
}
