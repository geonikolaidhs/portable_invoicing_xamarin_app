using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;


namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum PriceCatalogSearchMethod
    {
        [WrmDisplay(Name = "CurrentPriceCatalog", ResourceType = typeof(ResourcesRest))]
        CURRENT_PRICECATALOG,
        [WrmDisplay(Name = "PriceCatalogTree", ResourceType = typeof(ResourcesRest))]
        PRICECATALOG_TREE
    }
}
