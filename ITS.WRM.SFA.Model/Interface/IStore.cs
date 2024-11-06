using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IStore
    {
        string Name { get; set; }
        Guid? CentralOid { get; set; }
        ICompanyNew Owner { get; set; }
        Guid ImageOid { get; set; }
        IAddress Address { get; set; }
        bool IsCentralStore { get; set; }
        IPriceCatalog DefaultPriceCatalog { get; set; }
        string Code { get; set; }
        Guid ReferenceCompanyOid { get; set; }
    }
}
