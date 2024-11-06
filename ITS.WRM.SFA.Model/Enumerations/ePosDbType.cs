using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ITS.WRM.SFA.Model.Enumerations
{
    [Flags]
    public enum ePOSDbType
    {

        POS_SETTINGS = 1,
        POS_MASTER = 32,
        POS_TRANSCATION = 35,
        POS_VERSION = 39
    }
}