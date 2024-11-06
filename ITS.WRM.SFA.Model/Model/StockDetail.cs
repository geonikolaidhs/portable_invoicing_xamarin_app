using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Model
{
    public class StockDetail : BaseObj
    {
        public Guid ItemOid { get; set; }

        public Guid StockHeaderOid { get; set; }

        public decimal Qty { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }
    }
}
