using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPermission: IBasicObj
    {
        bool Visible { get; set; }
        bool CanInsert { get; set; }
        bool CanUpdate { get; set; }
        bool CanDelete { get; set; }
        bool CanExport { get; set; }
    }
}
