using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLite;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 21, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class TaxOffice : LookUp2Fields
    {

        public string Municipality { get; set; }

        public string PostCode { get; set; }

        public string Street { get; set; }

    }
}
