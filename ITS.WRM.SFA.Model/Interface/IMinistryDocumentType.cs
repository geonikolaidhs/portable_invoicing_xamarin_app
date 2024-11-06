﻿using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IMinistryDocumentType:ILookUpFields
    {
        string Code { get; set; }
        string Title { get; set; }
        string ShortTitle { get; set; }
        eDocumentValueFactor DocumentValueFactor { get; set; }
        bool IsSupported { get; set; }
    }
}
