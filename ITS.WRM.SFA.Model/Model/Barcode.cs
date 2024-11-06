using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 270, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Barcode : BasicObj
    {
        [Indexed]
        public string Code { get; set; }

        //[Ignore]
        //[JsonIgnore]
        //[ManyToMany(typeof(ItemBarcode), "Item", "Barcodes", ReadOnly = true)]
        ////[ManyToMany(typeof(ItemBarcode), CascadeOperations = CascadeOperation.All)]
        //public List<ItemBarcode> Items { get; set; }

        //[Ignore]
        //[ForeignKey(typeof(Item))]
        //public Guid Item { get; set; }

        public MeasurementUnit MeasurementUnit(DatabaseLayer databaseLayer, CompanyNew owner)
        {
            ItemBarcode itemBarcode = this.ItemBarcode(databaseLayer, owner);
            return itemBarcode == null ? null : DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Where(x => x.Oid == itemBarcode.MeasurementUnitOid).FirstOrDefault();
        }

        private ItemBarcode _ItemBarcode = null;



        public ItemBarcode ItemBarcode(DatabaseLayer databaseLayer, CompanyNew Owner)
        {
            if (Owner == null)
            {
                throw new Exception("Barcode Owner Cannot be null");
            }
            if (_ItemBarcode == null)
            {
                _ItemBarcode = databaseLayer.GetItemBarcode(this.Oid, Owner.Oid).FirstOrDefault();
            }
            return _ItemBarcode;
            //return databaseLayer.GetEnumerable<ItemBarcode>().Where(itemBarcode => itemBarcode.BarcodeOid == this.Oid && itemBarcode.OwnerOid == Owner.Oid).FirstOrDefault();
        }
    }
}
