using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using SQLiteNetExtensions.Attributes;
using SQLite;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Interface;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 160, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class SpecialItem : LookUp2Fields, IRequiredOwner
    {
        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }
    }
}
