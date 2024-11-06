using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ITS.WRM.SFA.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;

using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 420, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Item : BasicObj, IRequiredOwner
    {
        public bool AcceptsCustomDescription { get; set; }

        public bool IsTax { get; set; }
        public bool DoesNotAllowDiscounts { get; set; }


        public Guid Buyer { get; set; }
        public void SetBuyer(Buyer buyer)
        {
            if (buyer == null)
            {
                this.Buyer = Guid.Empty;
            }
            else
            {
                this.Buyer = buyer.Oid;
            }
        }

        public Buyer GetBuyer(DatabaseLayer databaseLayer)
        {
            if (this.Buyer == Guid.Empty || this.Buyer == null)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<Buyer>(this.Buyer);
            }
        }

        public string Code { get; set; }

        [JsonIgnore]
        [IgnoreUpdateProperty]
        public double Stock { get; set; }

        public decimal ContentUnit { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public Barcode DefaultBarcode { get; set; }

        [ForeignKey(typeof(Barcode))]
        [Column("DefaultBarcode")]
        [JsonProperty(PropertyName = "DefaultBarcode.Oid")]
        public Guid DefaultBarcodeOid { get; set; }

        public Guid DefaultSupplier { get; set; }

        public void SetDefaultSupplier(Supplier buyer)
        {
            if (DefaultSupplier == null)
            {
                this.DefaultSupplier = Guid.Empty;
            }
            else
            {
                this.DefaultSupplier = buyer.Oid;
            }
        }

        public Supplier GetDefaultSupplier(DatabaseLayer databaseLayer)
        {
            if (this.DefaultSupplier == Guid.Empty || this.DefaultSupplier == null)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<Supplier>(this.DefaultSupplier);
            }
        }

        public string Description { get; set; }

        public DateTime InsertedDate { get; set; }

        public bool IsGeneralItem { get; set; }

        public double MaxOrderQty { get; set; }

        public double MinOrderQty { get; set; }

        public string Name { get; set; }

        public double OrderQty { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public CompanyNew Owner { get; set; }

        [ForeignKey(typeof(CompanyNew))]
        [Column("Owner")]
        [JsonProperty(PropertyName = "Owner.Oid")]
        public Guid OwnerOid { get; set; }


        [JsonIgnore]
        [Ignore]
        [ExpandProperty]
        public MeasurementUnit PackingMeasurementUnit { get; set; }

        [ForeignKey(typeof(MeasurementUnit))]
        [Column("PackingMeasurementUnit")]
        [JsonProperty(PropertyName = "PackingMeasurementUnit.Oid")]
        public Guid PackingMeasurementUnitOid { get; set; }


        public double PackingQty { get; set; }

        public decimal Points { get; set; }

        public decimal ReferenceUnit { get; set; }

        public string Remarks { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public VatCategory VatCategory { get; set; }

        [ForeignKey(typeof(VatCategory))]
        [Column("VatCategory")]
        [JsonProperty(PropertyName = "VatCategory.Oid")]
        public Guid VatCategoryOid { get; set; }

        [Ignore]
        ICompanyNew IOwner.Owner { get; }

        [Ignore]
        [JsonIgnore]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ItemBarcode> ItemBarcodes { get; set; }



        public Guid ItemExtraInfo { get; set; }

        public void SetItemExtraInfo(ItemExtraInfo itemExtraInfo)
        {
            if (itemExtraInfo == null)
            {
                this.ItemExtraInfo = Guid.Empty;
            }
            else
            {
                this.ItemExtraInfo = itemExtraInfo.Oid;
            }
        }

        public Guid MotherCode { get; set; }


        public eItemCustomPriceOptions CustomPriceOptions
        {
            get; set;
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ItemAnalyticTree> ItemAnalyticTrees
        {
            get; set;
        }


        [Ignore]
        [JsonIgnore]
        public Item MotherItem { get; set; }

        [JsonIgnore]
        public List<PriceCatalogDetail> PriceCatalogDetails = new List<PriceCatalogDetail>();


        [JsonIgnore]
        public List<BarcodeDetails> BarcodeDetails = new List<BarcodeDetails>();



        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<LinkedItem> LinkedItems { get; set; }


        [JsonIgnore]
        public List<LinkedItemsDetails> LinkedItemsDetails = new List<LinkedItemsDetails>();

    }
}
