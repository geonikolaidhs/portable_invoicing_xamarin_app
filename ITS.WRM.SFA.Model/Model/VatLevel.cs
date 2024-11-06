using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 35, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class VatLevel: LookUp2Fields
    {
        bool IsDefaultLevel { get; set; }
        
        
    }
}
