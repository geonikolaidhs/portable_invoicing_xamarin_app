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
using ITS.WRM.SFA.Model.Helpers;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;


namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 20, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Buyer : LookUp2Fields, IRequiredOwner
    {
        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Item> Items { get; set; }
    }
}
