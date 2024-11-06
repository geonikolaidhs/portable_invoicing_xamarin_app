using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;

using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLite;
using ITS.WRM.SFA.Model.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 265, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class MeasurementUnit : LookUp2Fields, IRequiredOwner
    {
       
        public eMeasurementUnitType MeasurementUnitType { get; set; }
        
        public bool SupportDecimal { get; set; }
        
        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }

    }
}
