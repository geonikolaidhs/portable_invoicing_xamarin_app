using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
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
    [CreateOrUpdaterOrder(Order = 290, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CategoryNode : Category, ICategoryNode
    {
        public Guid? ParentOid { get; set; }

        public string CategoryPath(DatabaseLayer databaseLayer)
        {
            string path = "";
            string OidString = "";
            if (this.ParentOid.HasValue)
            {
                CategoryNode parent = databaseLayer.GetCategoryNodeById(this.ParentOid);
                CalculateCategoryPath(databaseLayer, parent, ref path, ref OidString);
            }
            else
            {
                path = this.Description;
                OidString = this.Oid.ToString();
            }
            return path;
        }

        private void CalculateCategoryPath(DatabaseLayer databaseLayer, CategoryNode node, ref string path, ref string OidString)
        {
            if (node.ParentOid.HasValue)
            {
                path = String.Format("-> {0}{1}", node.Description, path);
                OidString = String.Format(", {0}{1}", this.Oid.ToString(), OidString);
                CategoryNode parent = databaseLayer.GetCategoryNodeById(node.ParentOid);
                CalculateCategoryPath(databaseLayer, parent, ref path, ref OidString);
            }
            else
            {
                path = node.Description + path;
                OidString = node.Oid.ToString() + OidString;
            }
        }
    }
}
