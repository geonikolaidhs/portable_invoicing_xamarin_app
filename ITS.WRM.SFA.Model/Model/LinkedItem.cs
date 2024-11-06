using ITS.WRM.SFA.Model.Enumerations;
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
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 510, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class LinkedItem:BaseObj
    {
        [Indexed]
        [ForeignKey(typeof(Item))]
        [Column("SubItem")]
        [JsonProperty(PropertyName = "SubItem.Oid")]
        public Guid SubItemOid { get; set; }

        [Ignore]
        [IgnoreExpand]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        public Item SubItem
        {
            set; get;
        }

        [Indexed]
        [ForeignKey(typeof(Item))]
        [Column("Item")]
        [JsonProperty(PropertyName = "Item.Oid")]
        public Guid ItemOid { get; set; }

        [Ignore]
        [IgnoreExpand]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        public Item Item
        {
            set;get;
        }

        
        
        public double QtyFactor { get; set; }

    }
}
