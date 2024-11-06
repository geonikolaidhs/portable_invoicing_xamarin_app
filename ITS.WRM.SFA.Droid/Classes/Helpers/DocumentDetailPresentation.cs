using ITS.WRM.SFA.Model.Model;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public class DocumentDetailPresentation : DocumentDetail
    {

        public string TotalVatAmountDescr { get; set; }
        public string QtyDescr { get; set; }
        public string PackingQtyDescr { get; set; }
        public string UnitPriceDescr { get; set; }
        public string UnitPriceDescrWithoutvat { get; set; }
        public string TotalDiscountAmountWithoutVATDescr { get; set; }
        public string TotalDiscountAmountWithVATDescr { get; set; }
        public string GrossTotalDescr { get; set; }
        public string GrossTotalDescrWithoutVat { get; set; }
        public string CodeDescr { get; set; }
        public string NameDescr { get; set; }
        public string DiscountDesc
        {
            get
            {
                return TotalDiscountAmountWithoutVATDescr + " , " + TotalDiscountAmountWithVATDescr;
            }
        }
    }
}
