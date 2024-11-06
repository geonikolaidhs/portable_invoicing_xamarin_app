
using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CreateOrUpdaterOrderAttribute : Attribute
    {
        public int Order;
        public eUpdateDirection Permissions = eUpdateDirection.MASTER_TO_SFA | eUpdateDirection.SFA_TO_MASTER;
        public bool IgnoreInsert;
    }
}
