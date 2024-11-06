using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 40, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Address : BasicObj
    {

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(AddressType))]
        [JsonIgnore]
        public AddressType AddressType { get; set; }

        [Indexed]
        [ForeignKey(typeof(AddressType))]
        [Column("AddressType")]
        [JsonProperty(PropertyName = "AddressType.Oid")]
        public Guid AddressTypeOid { get; set; }


        public AddressType GetAddressType(DatabaseLayer databaseLayer)
        {
            if (AddressType == null)
            {
                if (this.AddressTypeOid == Guid.Empty || this.AddressTypeOid == null)
                {
                    return null;
                }
                else
                {
                    AddressType = databaseLayer.GetById<AddressType>(this.AddressTypeOid);
                }
            }
            return AddressType;
        }




        public int? AutomaticNumbering { get; set; }

        public string City { get; set; }

        public Guid? DefaultPhoneOid { get; set; }

        public void SetDefaultPhone(Phone phone)
        {
            if (phone == null)
            {
                this.DefaultPhoneOid = null;
            }
            else
            {
                this.DefaultPhoneOid = phone.Oid;
            }
        }

        public string Email { get; set; }

        public bool IsDefault { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        public List<Phone> Phones
        {
            get; set;
        }
        public string POBox { get; set; }

        public string PostCode { get; set; }

        public string Profession { get; set; }

        public string Region { get; set; }

        [ExpandProperty]
        [JsonProperty(PropertyName = "Store.Oid")]
        public Guid Store { get; set; }

        public void SetStore(Store store)
        {
            if (store == null)
            {
                this.Store = Guid.Empty;
            }
            else
            {
                this.Store = store.Oid;
            }
        }

        public string Street { get; set; }

        [Indexed]
        [ForeignKey(typeof(Trader))]
        [JsonProperty(PropertyName = "Trader.Oid")]
        [Column("Trader")]
        public Guid TraderOid { get; set; }


        [ExpandProperty]
        [Ignore]
        [JsonIgnore]
        public Trader Trader { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [JsonIgnore]
        [ExpandProperty]
        [Ignore]
        public VatLevel VatLevel { get; set; }

        [ForeignKey(typeof(VatLevel))]
        [JsonProperty(PropertyName = "VatLevel.Oid")]
        [Column("VatLevel")]
        public Guid VatLevelOid { get; set; }

        public string FullDescription
        {
            get
            {
                string address = "";
                if (!String.IsNullOrEmpty(Street))
                {
                    if (!String.IsNullOrEmpty(address))
                    {
                        address += ", ";
                    }
                    address += Street;
                }
                if (!String.IsNullOrEmpty(City))
                {
                    if (!String.IsNullOrEmpty(address))
                    {
                        address += ", ";
                    }
                    address += City;
                }
                if (!String.IsNullOrEmpty(PostCode))
                {
                    if (!String.IsNullOrEmpty(address))
                    {
                        address += ", ";
                    }
                    address += PostCode;
                }
                return address;
            }
        }


        public string Description(DatabaseLayer databaseLayer)
        {

            AddressType TypeOfAddress = GetAddressType(databaseLayer);
            string address = "";
            if (TypeOfAddress != null && String.IsNullOrEmpty(TypeOfAddress.Description) == false)
            {
                address += TypeOfAddress.Description;
            }

            if (String.IsNullOrEmpty(Street) == false)
            {
                if (String.IsNullOrEmpty(address) == false)
                {
                    address += ", ";
                }
                address += Street;
            }


            if (String.IsNullOrEmpty(City) == false)
            {
                if (String.IsNullOrEmpty(address) == false)
                {
                    address += ", ";
                }
                address += City;
            }

            if (String.IsNullOrEmpty(PostCode) == false)
            {
                if (String.IsNullOrEmpty(address) == false)
                {
                    address += ", ";
                }
                address += PostCode;
            }

            if (String.IsNullOrEmpty(POBox) == false)
            {
                if (String.IsNullOrEmpty(address) == false)
                {
                    address += ", ";
                }
                address += POBox;
            }

            return address;
        }
        [Ignore]
        [JsonIgnore]
        public Phone CustomerDefaultPhone { get; set; }

        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;
    }
}
