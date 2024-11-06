using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using SQLiteNetExtensions.Attributes;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 268, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CouponCategory : LookUp2Fields
    {
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<CouponMask> CouponMask { get; set; }
    }
}
