using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 10, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Trader : BasicObj, ITrader
    {
        public string Code { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TaxCode { get; set; }

        public string TaxOffice { get; set; }

        public Guid? TaxOfficeLookUpOid { get; set; }

        public eTraderType TraderType { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Address> Addresses { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Customer> Customers { get; set; }
    }
}
