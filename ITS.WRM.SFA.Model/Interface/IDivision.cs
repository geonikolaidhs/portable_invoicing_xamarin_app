using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IDivision : ILookUpFields, IDivisionModel
    {
        List<IDocumentType> DocumentTypes { get; set; }
    }
}
