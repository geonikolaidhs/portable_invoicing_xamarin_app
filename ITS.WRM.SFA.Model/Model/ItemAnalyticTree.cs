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
    [CreateOrUpdaterOrder(Order = 450, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemAnalyticTree : BaseObj
    {

        [ForeignKey(typeof(CategoryNode))]
        [Column("Root")]
        [JsonProperty(PropertyName = "Root.Oid")]
        public Guid RootOid { get; set; }

        [Ignore]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        public CategoryNode Root
        {
            get; set;
        }



        [ForeignKey(typeof(CategoryNode))]
        [Column("Node")]
        [JsonProperty(PropertyName = "Node.Oid")]
        public Guid NodeOid { get; set; }


        [Ignore]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        public CategoryNode Node { get; set; }


        [Indexed]
        [ForeignKey(typeof(Item))]
        [Column("Object")]
        [JsonProperty(PropertyName = "Object.Oid")]
        public Guid ObjectOid { get; set; }

        [Ignore]
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        public Item Object
        {
            get; set;
        }

        public ItemCategory GetItemCategory(Guid Oid, DatabaseLayer databaselayer)
        {
            return databaselayer.GetItemCategoryById(Oid);

        }

        public CategoryNode GetCategoryNode(DatabaseLayer databaselayer)
        {
            return databaselayer.GetCategoryNodeById(this.NodeOid);

        }
    }
}
