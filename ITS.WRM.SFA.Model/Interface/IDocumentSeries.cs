using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentSeries
    {
        bool HasAutomaticNumbering { get; set;  }
        bool IsCancelingSeries { get; set; }
        IStore Store { get; set; }
        string Remarks { get; set; }
        string PrintedCode { get; set; }
        Guid? IsCanceledByOid { get; set; }
        IDocumentSequence DocumentSequence { get; set; }
        eModule eModule { get; set; }
        bool ShouldResetMenu { get; set; }
        IPOS POS { get; set; }
    }
}
