using ITS.WRM.SFA.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class ItemStockPresent
    {

        public Guid Oid { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public double Stock { get; set; }

        public double CommiteddQuantity { get; set; }


        public void CalculateQuantityOnOpenDocuments(List<DocumentHeader> headers)
        {
            decimal qty = 0;
            foreach (DocumentHeader header in headers)
            {
                foreach (DocumentDetail dtl in header.DocumentDetails)
                {
                    if (dtl.ItemOid == this.Oid)
                    {
                        qty = qty + dtl.Qty;
                    }
                }
            }
            this.CommiteddQuantity = (double)qty;
        }

        public string DisplayStock
        {
            get
            {
                return Stock.ToString().Replace(".", ",");
            }
        }

        public string DisplayCommiteddQuantity
        {
            get
            {
                return CommiteddQuantity.ToString().Replace(".", ",");
            }
        }

        public string ReserverdQtyColor
        {
            get
            {
                return CommiteddQuantity > Stock ? "#f30303" : "#000000";
            }
        }



    }
}
