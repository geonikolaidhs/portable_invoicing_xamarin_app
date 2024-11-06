using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IMeasurementUnit: ILookUp2Fields, IRequiredOwner
    {
         eMeasurementUnitType MeasurementUnitType { get; set; }
         bool SupportDecimal { get; set; }
    }
}
