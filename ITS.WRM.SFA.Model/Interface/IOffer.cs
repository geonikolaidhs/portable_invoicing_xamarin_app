using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IOffer: ILookUp2Fields
    {
        string Description2 { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        IPriceCatalog PriceCatalog { get; set; }
    }
}
