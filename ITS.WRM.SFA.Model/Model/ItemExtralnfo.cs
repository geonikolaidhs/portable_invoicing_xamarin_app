using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using SQLite;
using ITS.WRM.SFA.Model.Enumerations;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 421, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemExtraInfo : BasicObj
    {

        public string Description { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Ingredients { get; set; }


        [Ignore]
        public CompanyNew Owner
        {
            get
            {
                if (Item == null)
                {
                    return null;
                }
                return Item.Owner;
            }
        }
        public DateTime PackedAt { get; set; }

        [ForeignKey(typeof(Item))]
        [JsonProperty(PropertyName = "Item.Oid")]
        [Column("Item")]
        public Guid ItemOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public Item Item { get; set; }
    }
}
