using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using Newtonsoft.Json;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 140, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ReasonCategory : LookUp2Fields
    {
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        public List<Reason> Reasons { get; set; }

    }
}
