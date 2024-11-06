using ITS.WRM.SFA.Model.Enumerations;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class PriceSearchTraceStep
    {
        public int Number { get; set; }
        public PriceCatalogSearchMethod SearchMethod { get; set; }
        public string PriceCatalogDescription { get; set; }
    }
}
