using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IStoreDocumentSeriesType: IBasicObj
    {
        IDocumentSeries DocumentSeries { get; set; }
        ICustomer DefaultCustomer { get; set; }
        ISupplierNew DefaultSupplier { get; set; }
        IDocumentType DocumentType { get; set; }
        ICustomReport DefaultCustomReport { get; set; }
        int Duplicates { get; set; }
        UserType UserType { get; set; }
        eStoreDocumentType StoreDocumentType { get; set; }
        string MenuDescription { get; set; }
    }
}
