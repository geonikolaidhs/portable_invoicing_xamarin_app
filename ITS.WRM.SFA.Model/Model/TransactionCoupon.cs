using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
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
    [CreateOrUpdaterOrder(Order = 1053, Permissions = eUpdateDirection.NONE)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class TransactionCoupon : BaseObj
    {
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public Coupon Coupon { get; set; }

        [ForeignKey(typeof(Coupon))]
        [Column("Coupon")]
        [JsonProperty("Coupon.Oid")]
        public Guid CouponOid { get; set; }

        public string CouponCode { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public CouponMask CouponMask { get; set; }

        [ForeignKey(typeof(CouponMask))]
        [Column("CouponMask")]
        [JsonProperty("CouponMask.Oid")]
        public Guid CouponMaskOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public DocumentDetailDiscount DocumentDetailDiscount { get; set; }

        [ForeignKey(typeof(DocumentDetailDiscount))]
        [Column("DocumentDetailDiscount")]
        [JsonProperty("DocumentDetailDiscount.Oid")]
        public Guid DocumentDetailDiscountOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public DocumentHeader DocumentHeader { get; set; }

        [Indexed]
        [ForeignKey(typeof(DocumentHeader))]
        [Column("DocumentHeader")]
        [JsonProperty("DocumentHeader.Oid")]
        public Guid DocumentHeaderOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public DocumentPayment DocumentPayment { get; set; }

        [ForeignKey(typeof(DocumentPayment))]
        [Column("DocumentPayment")]
        [JsonProperty("DocumentPayment.Oid")]
        public Guid DocumentPaymentOid { get; set; }

        public bool IsCanceled { get; set; }
        
    }
}
