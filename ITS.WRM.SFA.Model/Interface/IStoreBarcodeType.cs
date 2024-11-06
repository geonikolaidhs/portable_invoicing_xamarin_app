using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IStoreBarcodeType: IBaseObj
    {
        IStore Store { get; set; }
        IBarcodeType BarcodeType { get; set; }
        ICompanyNew Owner { get; set; }
    }
}
