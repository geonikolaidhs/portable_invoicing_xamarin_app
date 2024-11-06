using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IAddress
    {
        string Email { get; set; }
        bool IsDefault { get; set; }
        IAddressType AddressType { get; set; }
        string Street { get; set; }
        string POBox { get; set; }
        string PostCode { get; set; }
        string City { get; set; }
        Guid? DefaultPhoneOid { get; set; }
        ITrader Trader { get; set; }
        IStore Store { get; set; }
        IVatLevel VatLevel { get; set; }
        int? AutomaticNumbering { get; set; }
        string Region { get; set; }
        string Profession { get; set; }
        List<IPhone> Phones { get; set; }
        string Lat { get; set; }
        string Lng { get; set; }
    }
}
