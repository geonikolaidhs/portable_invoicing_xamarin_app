using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System.Runtime.Serialization;

namespace ITS.Retail.Mobile.AuxilliaryClasses
{
    [DataContract]
    public enum eDocumentType
    {

        //value from warehouse
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobNone")]
        [EnumMember]
        NONE = 0,//ALL_TYPES = 0,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobOrder")]
        [EnumMember]
        ORDER = 1,
        //Do not remove: Required for wcf service (otherwise
        [EnumMember]
        EMPTY_2 = 2,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobInvoice")]
        [EnumMember]
        INVOICE = 3,//3,
        //Do not remove
        [EnumMember]
        EMPTY_4 = 4,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobReception")]
        [EnumMember]
        RECEPTION = 5,//3,//5,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobInventory")]
        [EnumMember]
        INVENTORY = 6,//4,//6,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobMatching")]
        [EnumMember]
        MATCHING = 7,//5,//7,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobPicking")]
        [EnumMember]
        PICKING = 8,//6,//8,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobTransfer")]
        [EnumMember]
        TRANSFER = 9,//7,//9,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobTag")]
        [EnumMember]
        TAG = 10,//8,//10,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobPriceCheck")]
        [EnumMember]
        PRICE_CHECK = 11,//9,//11,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobCompetition")]
        [EnumMember]
        COMPETITION = 12,//10,//12,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobPacking")]
        [EnumMember]
        PACKING = 13,//11,//13,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobPackingOrPicking")]
        [EnumMember]
        PACKINGORPICKING = 14,//12,//14
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobEslInventory")]
        [EnumMember]
        ESL_INV = 15,//13//ESL_INV = 13
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobInvoiceSales")]
        [EnumMember]
        INVOICE_SALES = 16, //Added at 07 01 2014 after a request

        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobDecomposition")]
        [EnumMember]
        DECOMPOSITION = 17, //Added at 10 01 2017 after a request

        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "MobComposition")]
        [EnumMember]
        COMPOSITION = 18 //Added at 10 02 2017 after a request
        //Copy from Datalogger
        // ALL_TYPES = 0,
        //ORDER = 1,
        //NULL2 = 2,
        //INVOICE = 3,
        //NULL4 = 4,
        //RECEPTION = 5,
        //INVENTORY = 6,
        //MATCHING = 7,
        //PICKING = 8,
        //TRANSFER = 9,
        //TAG = 10,
        //PRICE_CHECK = 11,
        //COMPETITION = 12,
        ///// <summary>
        ///// Not used in documents
        ///// </summary>
        //ESL_INV = 13

        //Copy from Warehouse
        //ALL_TYPES = 0,
        //ORDER = 1,
        //INVOICE = 3,
        //RECEPTION = 5,
        //INVENTORY = 6,
        //MATCHING = 7,
        //PICKING = 8,
        //TRANSFER = 9,
        //TAG = 10,
        //PRICE_CHECK = 11,
        //COMPETITION = 12,
        //PACKING = 13,
        //PACKINGORPICKING = 14
    }
}
