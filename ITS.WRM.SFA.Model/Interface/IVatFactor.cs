using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IVatFactor
    {
        Guid VatLevel { get; set; }
        Guid VatCategory { get; set; }
    }
}
