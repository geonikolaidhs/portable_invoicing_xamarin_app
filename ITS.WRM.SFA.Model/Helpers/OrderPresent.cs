using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class OrderPresent
    {
        public Guid Oid { get; set; }
        public Guid StatusOid { get; set; }
        public Guid TypeOid { get; set; }
        public string CompanyName { get; set; }
        public string CustomerCode { get; set; }
        public bool IsSynchronized { get; set; }
        public string IsSynchronizedString
        {
            get
            {
                if (IsSynchronized)
                {
                    return ResourcesRest.MsgYes;
                }
                else
                {
                    return ResourcesRest.MsgNo;
                }
            }
        }
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
        public string FinalizedDateString
        {
            get
            {
                if (FinalizedDate == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    return FinalizedDate.ToString("dd-MM-yyyy");
                }

            }
        }

        public DateTime FinalizedDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public decimal NetTotal { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal Discount { get; set; }

        public string NetTotalString
        {
            get
            {
                return NetTotal.ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
            }
        }

        public string GrossTotalString
        {
            get
            {
                return GrossTotal.ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
            }
        }

        public string DiscountString
        {
            get
            {
                return Discount.ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
            }
        }


        public long CreatedOnTicks { get; set; }

        public string StatusColor
        {
            get
            {
                return DependencyService.Get<ICrossPlatformMethods>().GetSendStatus() != StatusOid ? "#f30303" : "#000000";

            }
        }

        public string SynchronizedColor
        {
            get
            {
                return !IsSynchronized ? "#f30303" : "#000000";
            }
        }


    }
}
