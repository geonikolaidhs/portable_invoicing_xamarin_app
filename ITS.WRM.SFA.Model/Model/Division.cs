using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using SQLite;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 170, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Division : LookupField
    {

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        public List<DocumentType> DocumentTypes { get; set; }

        public eDivision Section { get; set; }
    }
}
