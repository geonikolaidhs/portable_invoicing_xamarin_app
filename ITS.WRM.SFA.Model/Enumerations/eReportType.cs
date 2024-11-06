using ITS.WRM.SFA.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eReportType
    {        
        [WrmDescription("General")]
        General,
        [WrmDescription("Document")]
        Document
    }
}
