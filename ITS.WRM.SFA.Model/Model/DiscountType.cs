using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using SQLite;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 195, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DiscountType : LookUp2Fields
    {

        public bool DiscardsOtherDiscounts { get; set; }
        [Ignore]
        public eDiscountType EDiscountType { get; set; }

        public bool IsHeaderDiscount { get; set; }

        public bool IsUnique { get; set; }

        public int Priority { get; set; }

    }
}
