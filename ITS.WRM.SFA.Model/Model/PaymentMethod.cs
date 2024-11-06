using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;

using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using SQLiteNetExtensions.Attributes;
using SQLite;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 185, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class PaymentMethod : LookUp2Fields
    {
        public bool CanExceedTotal { get; set; }
        
        public bool ForceEdpsOffline { get; set; }

        public bool GiveChange { get; set; }

        public bool IncreasesDrawerAmount { get; set; }
        
        public bool IsNegative { get; set; }
        
        public bool NeedsRatification { get; set; }

        public bool NeedsValidation { get; set; }
        
        public bool OpensDrawer { get; set; }

        
        [Ignore]
        public ePaymentMethodType PaymentMethodType { get; set; }
        
        [Column("PaymentMethodType")]
        public int PaymentMethodTypeOid { get; set; }

        public bool UseEDPS { get; set; }

        public bool UsesInstallments { get; set; }
        
    }
}
