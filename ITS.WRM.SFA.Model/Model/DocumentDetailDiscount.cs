using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using SQLite;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 1051, Permissions = eUpdateDirection.NONE)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class DocumentDetailDiscount : CustomFieldStorage
    {
        public string Description { get; set; }

        public bool DiscardsOtherDiscounts { get; set; }

        public eDiscountSource DiscountSource { get; set; }

        public eDiscountType DiscountType { get; set; }

        public decimal DiscountWithoutVAT { get; set; }

        public decimal DiscountWithVAT { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public DocumentDetail DocumentDetail { get; set; }

        [Indexed]
        [ForeignKey(typeof(DocumentDetail))]
        [Column("DocumentDetail")]
        [JsonProperty("DocumentDetail.Oid")]
        public Guid DocumentDetailOid { get; set; }

        public decimal Percentage { get; set; }

        /// <summary>
        /// Calculated
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public string DiscountPercentagePrint
        {
            get
            {
                return String.Format("{0:P2}.", Percentage);
            }
        }

        public int Priority { get; set; }

        public Guid Promotion { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public TransactionCoupon TransactionCoupon { get; set; }

        [ForeignKey(typeof(TransactionCoupon))]
        [Column("TransactionCoupon")]
        [JsonProperty("TransactionCoupon.Oid")]
        public Guid TransactionCouponOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public DiscountType Type { get; set; }

        [ForeignKey(typeof(DiscountType))]
        [Column("Type")]
        [JsonProperty("Type.Oid")]
        public Guid DiscountTypeOid { get; set; }

        public string TypeDescription { get; set; }

        public decimal Value { get; set; }


        public string DisplayName
        {
            get
            {
                string displayName = "";
                if (String.IsNullOrWhiteSpace(this.Description) == false)
                {
                    displayName = this.Description;
                }
                else if (String.IsNullOrWhiteSpace(this.TypeDescription) == false)
                {
                    displayName = this.TypeDescription;
                }
                else
                {
                    displayName = ResourcesRest.Discount.ToUpper();
                }

                return displayName;
            }
        }


    }
}

