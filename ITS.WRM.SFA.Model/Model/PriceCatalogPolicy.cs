using ITS.WRM.SFA.Model.Interface;

using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;
using SQLite;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 46, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalogPolicy : LookUp2Fields, IRequiredOwner
    {
        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }

        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        public List<PriceCatalogPolicyDetail> PriceCatalogPolicyDetails { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        public List<StorePriceCatalogPolicy> StrorePriceCatalogPolicy { get; set; }
    }
}
