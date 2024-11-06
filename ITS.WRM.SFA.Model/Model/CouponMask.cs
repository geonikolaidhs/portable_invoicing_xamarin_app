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
    [CreateOrUpdaterOrder(Order = 269, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CouponMask : CouponBase
    {
        [ForeignKey(typeof(CouponCategory))]
        public Guid CouponCategory { get; set; }


        public string Mask { get; set; }
        public string Prefix { get; set; }
        public string PropertyName { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All), JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public PaymentMethod PaymentMethod { get; set; }

        [ForeignKey(typeof(PaymentMethod))]
        [JsonProperty(PropertyName = "PaymentMethod.Oid")]
        [Column("PaymentMethod")]
        public Guid PaymentMethodOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All), JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public DiscountType DiscountType { get; set; }

        [ForeignKey(typeof(DiscountType))]
        [JsonProperty(PropertyName = "DiscountType.Oid")]
        [Column("DiscountType")]
        public Guid DiscountTypeOid { get; set; }



    }
}
