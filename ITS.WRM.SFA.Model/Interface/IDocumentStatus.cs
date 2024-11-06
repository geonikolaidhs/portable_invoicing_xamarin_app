using ITS.WRM.SFA.Model.Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentStatus: ILookUp2Fields, IRequiredOwner
    {
         bool TakeSequence { get; set; }
         bool ReadOnly { get; set; }
         List<IDocumentHeader> DocumentHeaders { get; set; }
         
    }
}
