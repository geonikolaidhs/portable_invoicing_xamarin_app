using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPriceCatalog: ILookUp2Fields
    {
        DateTime StartDate { get; set; }
        bool SupportLoyalty { get; set; }
        IStore IsEditableAtStore { get; set; }
        DateTime EndDate { get; set; }
        Guid? ParentCatalogOid { get; set; }
        bool IsRoot { get; set; }
        int Level { get; set; }
        bool IgnoreZeroPrices { get; set; }
    }
}
