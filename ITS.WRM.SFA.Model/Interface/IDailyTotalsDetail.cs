using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IDailyTotalsDetail
    {
        //IDailyTotals DailyTotals { get; set; }
        IVatFactor VatFactor { get; set; }
        IPaymentMethod Payment { get; set; }
    }
}
