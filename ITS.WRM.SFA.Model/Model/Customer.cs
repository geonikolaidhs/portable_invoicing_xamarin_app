using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.NonPersistant;
using SQLite;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 210, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Customer : BasicObj
    {
        public double Balance { get; set; }
        public long BirthDateTicks { get; set; }
        public bool BreakOrderToCentral { get; set; }
        public string CardID { get; set; }
        public decimal Cats { get; set; }
        public string Code { get; set; }
        public decimal CollectedPoints { get; set; }
        public string CompanyBrandName { get; set; }
        public string CompanyName { get; set; }
        [Ignore]
        public List<ICustomerAnalyticTree> CustomerAnalyticTrees { get; set; }
        [Ignore]
        public List<ICustomerStorePriceList> CustomerStorePriceLists { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(Address))]
        [JsonIgnore]
        public Address DefaultAddress { get; set; }

        [Indexed]
        [ForeignKey(typeof(Address))]
        [Column("DefaultAddress")]
        [JsonProperty(PropertyName = "DefaultAddress.Oid")]
        public Guid DefaultAddressOid { get; set; }


        public Address GetDefaultAddress(DatabaseLayer databaseLayer)
        {
            if (DefaultAddress == null)
            {
                if (this.DefaultAddressOid == Guid.Empty || this.DefaultAddressOid == null)
                {
                    List<Address> addressList = databaseLayer.GetAddressByTrader(TraderOid).ToList();
                    if (addressList.Count == 1)
                    {
                        DefaultAddress = addressList.First();
                    }
                }
                else
                {
                    DefaultAddress = databaseLayer.GetById<Address>(this.DefaultAddressOid);
                }
            }
            return DefaultAddress;
        }

        [Indexed]
        public string Description { get; set; }
        public double Discount { get; set; }
        public decimal Dogs { get; set; }
        public string Email { get; set; }
        public string FatherName { get; set; }
        public string Loyalty { get; set; }
        public eMaritalStatus MaritalStatus { get; set; }
        public decimal OtherPets { get; set; }
        [Indexed]
        [ForeignKey(typeof(VatLevel))]
        [Column("Owner")]
        [JsonProperty(PropertyName = "Owner.Oid")]
        public Guid OwnerOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(CompanyNew))]
        [JsonIgnore]
        public CompanyNew Owner { get; set; }

        public void SetOwner(CompanyNew owner)
        {
            if (owner == null)
            {
                this.Owner = null;
                this.OwnerOid = Guid.Empty;
            }
            this.Owner = owner;
            this.OwnerOid = owner.Oid;
        }

        public CompanyNew GetOwner(DatabaseLayer databaseLayer)
        {
            if (this.OwnerOid == Guid.Empty)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<CompanyNew>(this.OwnerOid);
            }
        }

        [JsonProperty(PropertyName = "PaymentMethod.Oid")]
        [ExpandProperty]
        public Guid PaymentMethod { get; set; }

        public void SetPaymentMethod(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
            {
                this.PaymentMethod = Guid.Empty;
            }
            else
            {
                this.PaymentMethod = paymentMethod.Oid;
            }
        }

        public PaymentMethod GetPaymentMethod(DatabaseLayer databaseLayer)
        {
            if (this.PaymentMethod == Guid.Empty)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<PaymentMethod>(this.PaymentMethod);
            }
        }

        [JsonProperty(PropertyName = "PriceCatalogPolicy.Oid")]
        [ExpandProperty]
        public Guid PriceCatalogPolicy { get; set; }

        public void SetPriceCatalogPolicy(PriceCatalogPolicy priceCatalogPolicy)
        {
            if (priceCatalogPolicy == null)
            {
                this.PriceCatalogPolicy = Guid.Empty;
            }
            else
            {
                this.PriceCatalogPolicy = priceCatalogPolicy.Oid;
            }
        }

        public PriceCatalogPolicy GetPriceCatalogPolicy(DatabaseLayer databaseLayer)
        {
            if (this.PriceCatalogPolicy == Guid.Empty)
            {
                return null;
            }
            else
            {
                return databaseLayer.GetById<PriceCatalogPolicy>(this.PriceCatalogPolicy);
            }
        }

        public string Profession { get; set; }

        [ExpandProperty]
        [JsonProperty(PropertyName = "RefundStore.Oid")]
        public Guid RefundStore { get; set; }

        public void SetRefundStore(Store store)
        {
            if (store == null)
            {
                this.RefundStore = Guid.Empty;
            }
            else
            {
                this.RefundStore = store.Oid;
            }
        }

        public ITS.WRM.SFA.Model.Enumerations.eSex Sex { get; set; }
        public decimal TotalConsumedPoints { get; set; }
        public decimal TotalEarnedPoints { get; set; }

        [Indexed]
        [ForeignKey(typeof(Trader))]
        [JsonProperty(PropertyName = "Trader.Oid")]
        [Column("Trader")]
        [ComplexSerialise(SerialiseProperty = "Trader")]
        public Guid TraderOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public Trader Trader { get; set; }

        public void SetTrader(Trader trader)
        {
            if (trader == null)
            {
                this.Trader = null;
                this.TraderOid = Guid.Empty;
            }
            else
            {
                this.Trader = trader;
                this.TraderOid = trader.Oid;
            }

        }


        [Indexed]
        [ForeignKey(typeof(VatLevel))]
        [JsonProperty(PropertyName = "VatLevel.Oid")]
        [Column("VatLevel")]
        public Guid VatLevelOid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All), JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public VatLevel VatLevel { get; set; }


        public VatLevel GetVatLevel(Address address)
        {
            if (address != null && address.TraderOid == this.TraderOid && address.VatLevelOid != null && address.VatLevelOid != Guid.Empty)
            {
                return DependencyService.Get<ICrossPlatformMethods>().GetVatLevels().Where(x => x.Oid == address.VatLevelOid).FirstOrDefault();
            }
            return VatLevel;
        }

        [Ignore]
        [JsonIgnore]
        public string FullAddressDescription { get; set; }

        [Ignore]
        [JsonIgnore]
        public string CustomerDefaultPhone { get; set; }
    }
}
