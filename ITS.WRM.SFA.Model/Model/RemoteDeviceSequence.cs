using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{ 
    [CreateOrUpdaterOrder(Order = 1, Permissions = eUpdateDirection.NONE)]
    public class RemoteDeviceSequence: BasicObj
    {
        public int RemoteDeviceSequenceNumber { get; set; }
    }
}
