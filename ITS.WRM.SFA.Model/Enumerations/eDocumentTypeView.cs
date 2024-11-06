using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDocumentTypeView
    {
        [WrmDisplay(Name = "SimpleFormView", ResourceType = typeof(ResourcesRest))]
        Simple,
        [WrmDisplay(Name = "AdvancedFormView", ResourceType = typeof(ResourcesRest))]
        Advanced,
        [WrmDisplay(Name = "CompositionDecomposition", ResourceType = typeof(ResourcesRest))]
        CompositionDecomposition
    }
}
