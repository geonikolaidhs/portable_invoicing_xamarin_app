using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface ISupplierNew
    {
        ITrader Trader { get; set; }
        string Code { get; set; }
        string CompanyName { get; set; }
        IAddress DefaultAddress { get; set; }
        string Profession { get; set; }
        ICompanyNew Owner { get; set; }
        IVatLevel VatLevel { get; set; }
        List<IItem> Items { get; set;  }
        //List<ISupplierImportFilesSet> SupplierImportFilesSets { get; set; }

    }
}
