using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class BarcodeDetails
    {
        public Guid BarcodeOid { get; set; }

        public string MeasurementUnit { get; set; }

        public string Barcode { get; set; }


        public DateTime CreatedOn
        {
            get
            {
                return new DateTime(CreatedOnTicks);
            }
            set
            {
                if (value == null)
                {
                    CreatedOnTicks = DateTime.MinValue.Ticks;
                }
                else
                {
                    CreatedOnTicks = value.Ticks;
                }
            }

        }

        public DateTime UpdatedOn
        {
            get
            {
                return new DateTime(UpdatedOnTicks);
            }
            set
            {
                if (value == null)
                {
                    UpdatedOnTicks = DateTime.MinValue.Ticks;
                }
                else
                {
                    UpdatedOnTicks = value.Ticks;
                }
            }

        }

        private long UpdatedOnTicks { get; set; }
        private long CreatedOnTicks { get; set; }

    }
}
