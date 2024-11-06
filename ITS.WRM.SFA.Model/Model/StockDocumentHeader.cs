using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Model
{
    public class StockDocumentHeader : BaseObj
    {

        public bool Executed { get; set; }

        public long DateCreatedTicks { get; set; }

        public string Description { get; set; }

        public int Number { get; set; }

        public decimal TotalQty { get; set; }

        public List<StockDetail> Details = new List<StockDetail>();

        [JsonIgnore]
        [Ignore]
        public bool Selected { get; set; }

        [JsonIgnore]
        [Ignore]
        public string CreatedDate
        {
            get
            {
                DateTime dt = new DateTime(CreatedOnTicks);
                if (dt == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    return dt.ToString("dd-MM-yyyy");
                }
            }
        }

        [JsonIgnore]
        [Ignore]
        public string PresentStatus
        {
            get
            {
                return Executed ? ResourcesRest.Executed : ResourcesRest.NonExecuted;
            }
        }


        [JsonIgnore]
        [Ignore]
        public string StatusColor
        {
            get
            {
                return Executed ? "#000000" : "#f30303";
            }
        }
    }
}
