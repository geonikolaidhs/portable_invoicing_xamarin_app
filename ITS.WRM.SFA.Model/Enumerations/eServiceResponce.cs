using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    /// <summary>
    /// !!! ATTENTION !!!
    /// This should be a value that can be stored into a byte!!!
    /// </summary>
    public enum eServiceResponce
    {
        SUCCESS = 0,
        INVALID_INPUT,
        EMPTY_RESPONCE,
        EXCEPTION_HAS_BEEN_THROWN = 200,
        GENERAL_FAILURE = 255
    }
}
