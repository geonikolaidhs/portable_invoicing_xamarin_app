using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using SQLite;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    //[CreateOrUpdaterOrder(Order = 270, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Coupon : CouponBase
    {
        public decimal Amount { get; set; }

        public string Code { get; set; }
        [ManyToOne, JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public CouponCategory CouponCategory { get; set; }

        [ForeignKey(typeof(CouponCategory))]
        [JsonProperty(PropertyName = "CouponCategory.Oid")]
        public Guid CouponCategoryOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.None)]
        [ExpandProperty]
        [Ignore]
        public CouponMask CouponMask { get; set; }
        [ForeignKey(typeof(CouponMask))]
        [JsonProperty(PropertyName = "CouponMask.Oid")]
        public Guid CouponMaskOid { get; set; }

        public int NumberOfTimesUsed { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public CouponAmountIsAppliedAs CouponAmountIsAppliedAs { get; set; }

        [ForeignKey(typeof(CouponAmountIsAppliedAs))]
        [JsonProperty(PropertyName = "CouponAmountIsAppliedAs.Oid")]
        public Guid CouponAmountIsAppliedAsOid { get; set; }


        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public CouponAppliesOn CouponAppliesOn { get; set; }

        [ForeignKey(typeof(CouponAppliesOn))]
        [JsonProperty(PropertyName = "CouponAppliesOn.Oid")]
        public Guid CouponAppliesOnOid { get; set; }



    }
}
