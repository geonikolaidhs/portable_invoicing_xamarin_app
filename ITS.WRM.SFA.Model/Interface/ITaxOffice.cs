using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface ITaxOffice: ILookUp2Fields
    {
        string Street { get; set; }
        string PostCode { get; set; }
        string Municipality { get; set; }
    }
}
