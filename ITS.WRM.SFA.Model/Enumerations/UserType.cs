using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum UserType
    {
        [WrmDisplay(Name = "CompanyUser", ResourceType = typeof(ResourcesRest))]
        COMPANYUSER,
        [WrmDisplay(Name = "Trader", ResourceType = typeof(ResourcesRest))]
        TRADER,
        [WrmDisplay(Name = "AllUserType", ResourceType = typeof(ResourcesRest))]
        ALL,
        [WrmDisplay(Name = "Admin", ResourceType = typeof(ResourcesRest))]
        ADMIN,
        [WrmDisplay(Name = "NoneUserType", ResourceType = typeof(ResourcesRest))]
        NONE
    }
}