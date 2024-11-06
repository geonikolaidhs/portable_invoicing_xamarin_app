using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IRelativeDocumentDetail : IBaseObj
    {
        IDocumentDetail DerivedDocumentDetail { get; set; }
        decimal Qty { get; set; }
        IDocumentDetail InitialDocumentDetail { get; set; }
        IRelativeDocument RelativeDocument { get; set; }
    }
}
