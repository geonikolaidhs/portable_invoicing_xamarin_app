using ITS.WRM.SFA.Resources;
using ITS.WRM.SFA.Model.Attributes;


namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum ComparisonOperator
    {
        [WrmDisplay(Name = "EQUAL", ResourceType = typeof(ResourcesRest))]
        EQUAL,
        [WrmDisplay(Name = "NOT_EQUAL", ResourceType = typeof(ResourcesRest))]
        NOT_EQUAL,
        [WrmDisplay(Name = "GREATER_THAN", ResourceType = typeof(ResourcesRest))]
        GREATER_THAN,
        [WrmDisplay(Name = "GREATER_OR_EQUAL", ResourceType = typeof(ResourcesRest))]
        GREATER_OR_EQUAL,
        [WrmDisplay(Name = "LESS_THAN", ResourceType = typeof(ResourcesRest))]
        LESS_THAN,
        [WrmDisplay(Name = "LESS_OR_EQUAL", ResourceType = typeof(ResourcesRest))]
        LESS_OR_EQUAL
    }
}
