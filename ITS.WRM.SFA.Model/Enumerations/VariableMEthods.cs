using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum VariableMethods
    {
        [WrmDisplay(Name = "NoneFemale", ResourceType = typeof(ResourcesRest))]
        NONE,
        [WrmDisplay(Name = "INCREASE", ResourceType = typeof(ResourcesRest))]
        INCREASE,
        [WrmDisplay(Name = "DECREASE", ResourceType = typeof(ResourcesRest))]
        DECREASE
    }
}
