using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class LookUp2Fields : BaseObj
    {
        public string Code { get; set; }



        public string Description { get; set; }


        public bool IsDefault { get; set; }



        //public CompanyNew Owner { get; set; }

        public string ReferenceCode { get; set; }


        public bool Update { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [ForeignKey(typeof(CompanyNew))]
        [Column("Owner")]
        [JsonProperty(PropertyName = "Owner.Oid")]
        public Guid OwnerOid { get; set; }

    }
}
