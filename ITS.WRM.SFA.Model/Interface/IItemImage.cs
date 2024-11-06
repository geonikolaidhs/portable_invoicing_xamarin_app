using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IItemImage : ILookUpFields
    {
        int ImageWidth { get; set; }
        int ImageHeight { get; set; }
        string Info { get; set; }
        Guid ItemOid { get; set; }
        //Image Image { get; set; }
        byte[] ImageSqlite { get; set; }
    }
}
