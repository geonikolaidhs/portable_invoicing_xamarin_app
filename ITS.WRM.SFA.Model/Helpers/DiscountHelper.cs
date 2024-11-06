using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class DiscountHelper
    {
        public DocumentDetailDiscount CreatePriceCatalogDetailDiscount(decimal priceCatalogDiscount)
        {
            DocumentDetailDiscount pcDiscount = new DocumentDetailDiscount()
            {
                Percentage = priceCatalogDiscount,
                Priority = -1,
                DiscountSource = eDiscountSource.PRICE_CATALOG,
                DiscountType = eDiscountType.PERCENTAGE
            };
            return pcDiscount;
        }
    }
}
