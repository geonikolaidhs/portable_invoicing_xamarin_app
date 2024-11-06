using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 460, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemBarcode : BaseObj, IRequiredOwner
    {

        public string Description { get; set; }

        public string PluCode { get; set; }

        public string PluPrefix { get; set; }

        public double RelationFactor { get; set; }

        [Indexed]
        [Column("Item")]
        [JsonProperty("Item.Oid")]
        [ForeignKey(typeof(Item))]
        public Guid ItemOid { get; set; }


        [Indexed]
        [Column("Barcode")]
        [ForeignKey(typeof(Barcode))]
        [JsonProperty("Barcode.Oid")]
        public Guid BarcodeOid { get; set; }


        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Barcode Barcode { get; set; }

        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Item Item { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [JsonProperty("Owner.Oid")]
        [Column("Owner")]
        [ForeignKey(typeof(CompanyNew))]
        public Guid OwnerOid { get; set; }

        [ForeignKey(typeof(BarcodeType))]
        [JsonProperty("Type.Oid")]
        [Column("Type")]
        public Guid TypeOid { get; set; }

        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public BarcodeType Type { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public MeasurementUnit MeasurementUnit { get; set; }

        [JsonProperty("MeasurementUnit.Oid")]
        [Column("MeasurementUnit")]
        [ForeignKey(typeof(MeasurementUnit))]
        public Guid MeasurementUnitOid { get; set; }

        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }

    }
}
