using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 1, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class SFA : BasicObj
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public string GoogleApiKey { get; set; }

    }
}
