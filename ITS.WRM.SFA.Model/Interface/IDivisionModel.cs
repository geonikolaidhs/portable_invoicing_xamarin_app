﻿using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IDivisionModel : ILookUpFields
    {
        eDivision Section { get; set; }
    }
}
