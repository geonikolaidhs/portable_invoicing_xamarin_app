using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IReasonCategory : ILookUp2Fields
    {


        List<IReason> Reasons
        {
            get;
            set;
        }
    }
}
