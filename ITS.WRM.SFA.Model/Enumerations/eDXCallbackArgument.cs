using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDXCallbackArgument
    {
        UNKNOWN,
        STARTEDIT,
        ADDNEWROW,
        CANCELEDIT,
        DELETESELECTED,
        SELECTROWS,
        PAGERONCLICK,
        COLUMNMOVE,
        SORT,
        APPLYCOLUMNFILTER, 
        APPLYFILTER
    }
}
