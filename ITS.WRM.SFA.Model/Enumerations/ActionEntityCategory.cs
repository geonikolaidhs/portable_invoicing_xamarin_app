using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum ActionEntityCategory
    {
        [WrmDisplay(Name = "Document", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader")]
        Document,
        [WrmDisplay(Name = "Item", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader", "DocumentDetails.Item")]
        DocumentItem,
        [WrmDisplay(Name = "ItemStock", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader", "DocumentDetails.Item.ItemStocks", "", "Store")]
        DocumentItemStock,
        [WrmDisplay(Name = "Customer", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader", "Customer")]
        DocumentCustomer,
        [WrmDisplay(Name = "Supplier", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader", "Supplier")]
        DocumentSupplier,
        [WrmDisplay(Name = "DocumentDetails", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader", "DocumentDetails", "IsCanceled = false")]
        DocumentDetails,
        [WrmDisplay(Name = "PaymentMethod", ResourceType = typeof(ResourcesRest)), ActionTypeInfo("DocumentHeader", "DocumentPayments")]
        DocumentPaymentMethod
    }
}
