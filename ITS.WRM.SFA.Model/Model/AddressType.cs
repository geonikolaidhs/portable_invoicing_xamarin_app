using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 30, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class AddressType: LookUp2Fields
    {
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Address> Addresss { get; set; }
    }
}
