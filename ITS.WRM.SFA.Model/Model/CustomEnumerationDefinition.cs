using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 192, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class CustomEnumerationDefinition : LookUp2Fields
    {

    }
}
