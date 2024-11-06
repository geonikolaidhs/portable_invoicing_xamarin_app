using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 620, Permissions = eUpdateDirection.SFA_TO_MASTER)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class DocumentPayment : CustomFieldStorage
    {
        public decimal Amount { get; set; }

        public string DisplayAmount
        {
            get
            {
                return Amount.ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR")) + " ";
            }
        }
        //public string DisplayName
        //{
        //    get
        //    {
        //        return IsChange ? Resources.ResourcesRest.Change : PaymentMethod.Description;
        //    }
        //}

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

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public PaymentMethod PaymentMethod { get; set; }

        [ForeignKey(typeof(PaymentMethod))]
        [Column("PaymentMethod")]
        [JsonProperty("PaymentMethod.Oid")]
        public Guid PaymentMethodOid { get; set; }

        public string PaymentMethodCode { get; set; }

        [JsonIgnore]
        public bool IsChange { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public TransactionCoupon TransactionCoupon { get; set; }

        [ForeignKey(typeof(TransactionCoupon))]
        [Column("TransactionCoupon")]
        [JsonProperty("TransactionCoupon.Oid")]
        public Guid TransactionCouponOid { get; set; }

        public string PaymentMethodDescription { get; set; }



    }
}
