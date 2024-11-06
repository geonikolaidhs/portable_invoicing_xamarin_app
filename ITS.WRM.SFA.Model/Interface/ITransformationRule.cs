
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.WRM.SFA.Model.Enumerations;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface ITransformationRule: IOwner, IBaseObj
    {
        bool IsDefault { get; set; }
        eTransformationLevel TransformationLevel { get; set; }
        double ValueTransformationFactor { get; set; }
        IDocumentType InitialType { get; set; }
        IDocumentType DerrivedType { get; set; }
        double QtyTransformationFactor { get; set; }
    }
}
