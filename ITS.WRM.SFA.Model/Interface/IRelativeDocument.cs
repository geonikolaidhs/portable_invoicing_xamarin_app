﻿using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IRelativeDocument: IBaseObj
    {
        IDocumentHeader DerivedDocument { get; set; }        
        IDocumentHeader InitialDocument { get; set; }
    }
}
