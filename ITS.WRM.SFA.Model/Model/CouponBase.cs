using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model.NonPersistant;

using ITS.WRM.SFA.Model.Attributes;
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
    public class CouponBase : BaseObj, IRequiredOwner
    {
        public long IsActiveUntil { get; set; }
        public long IsActiveFrom { get; set; }
        public bool IsUnique { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        public ICompanyNew Owner { get; set; }

        [JsonProperty(PropertyName = "Owner.Oid")]
        [Column("Owner")]
        [ForeignKey(typeof(CompanyNew))]
        public Guid OwnerOid { get; set; }

        public string Description { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public CouponAppliesOn CouponAppliesOn { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public CouponAmountIsAppliedAs CouponAmountIsAppliedAs { get; set; }


    }
}
