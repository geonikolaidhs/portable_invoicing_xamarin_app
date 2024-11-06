using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface ICustomerStorePriceList : IOwner, IBaseObj
    {
        int Gravity { get; set; }
        ICustomer Customer { get; set; }
        IStorePriceList StorePriceList { get; set; }
        bool IsDefault { get; set; }
    }
}
