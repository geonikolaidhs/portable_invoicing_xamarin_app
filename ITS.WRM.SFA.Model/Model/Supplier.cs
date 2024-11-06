using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Interface;
using Xamarin.Forms;
using ITS.WRM.SFA.Model.Helpers;
using Newtonsoft.Json;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 410, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Supplier : BaseObj
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public Address DefaultAddress { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public Item Items { get; set; }
        [Ignore]
        public ICompanyNew Owner { get; set; }
        public string Profession { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]

        [Indexed]
        [ForeignKey(typeof(Trader))]
        [JsonProperty(PropertyName = "Trader.Oid")]
        [Column("Trader")]
        [ComplexSerialise(SerialiseProperty = "Trader")]
        public Guid TraderOid { get; set; }


        [Indexed]
        [ForeignKey(typeof(VatLevel))]
        [JsonProperty(PropertyName = "VatLevel.Oid")]
        [Column("VatLevel")]
        public Guid VatLevelOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All), JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public VatLevel VatLevel
        {
            get => VatLevel == null ? DependencyService.Get<ICrossPlatformMethods>().GetVatLevels().Where(x => x.Oid == VatLevelOid).FirstOrDefault() : VatLevel;
            set => VatLevel = value;
        }






        public VatLevel GetVatLevel(Address address)
        {
            if (address != null && address.TraderOid == this.TraderOid && address.VatLevelOid != null && address.VatLevelOid != Guid.Empty)
            {
                return DependencyService.Get<ICrossPlatformMethods>().GetVatLevels().Where(x => x.Oid == address.VatLevelOid).FirstOrDefault();
            }
            return VatLevel;
        }
    }
}
