using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using SQLiteNetExtensions.Attributes;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;


namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 70, Permissions = eUpdateDirection.MASTER_TO_SFA)]

    public class VatCategory : LookUp2Fields
    {
        public eMinistryVatCategoryCode MinistryVatCategoryCode { get; set; }

    }
}
