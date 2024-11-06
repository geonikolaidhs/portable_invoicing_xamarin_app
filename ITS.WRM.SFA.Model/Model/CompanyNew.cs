using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 20, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class CompanyNew : BasicObj, ICompanyNew
    {
        public string B2CURL { get; set; }

        public double Balance { get; set; }

        public string Code { get; set; }

        [Ignore]
        [JsonIgnore]
        private OwnerApplicationSettings OwnerApplicationSettings { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public Trader Trader { get; set; }

        [ForeignKey(typeof(Trader))]
        [JsonProperty(PropertyName = "Trader.Oid")]
        [Column("Trader")]
        public Guid TraderOid { get; set; }

        public string CompanyName { get; set; }

        [Column("DefaultAddress")]
        public Guid? DefaultAddressOid { get; set; }

        public string Profession { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Item> Items { get; set; }


        IOwnerApplicationSettings ICompanyNew.OwnerApplicationSettings
        {
            get;

            set;

        }

        ITrader ICompanyNew.Trader
        {
            get
            {
                return this.Trader as ITrader;
            }

            set
            {
                this.Trader = value as Trader;
            }
        }
        public OwnerApplicationSettings GetOwnerApplicationSettings(DatabaseLayer databaseLayer)
        {
            if (this.OwnerApplicationSettings == null)
            {
                this.OwnerApplicationSettings = databaseLayer.GetOwnerApplicationSettings(this.Oid);
            }
            return this.OwnerApplicationSettings;
        }
    }
}
