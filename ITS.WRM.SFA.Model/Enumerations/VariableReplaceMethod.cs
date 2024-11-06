using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;


namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum VariableReplaceMethod
    {
        [WrmDisplay(Name = "Replace", ResourceType = typeof(ResourcesRest))]
        REPLACE,
        [WrmDisplay(Name = "Sum", ResourceType = typeof(ResourcesRest))]
        SUM
    }
}
