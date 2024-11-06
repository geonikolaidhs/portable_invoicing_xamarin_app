using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eRoleType
    {
        [WrmDisplay(Name = "Customer", ResourceType = typeof(ResourcesRest))]
        Customer = 0,
        [WrmDisplay(Name = "Supplier", ResourceType = typeof(ResourcesRest))]
        Supplier = 1,
        [WrmDisplay(Name = "CompanyUser", ResourceType = typeof(ResourcesRest))]
        CompanyUser = 2,
        [WrmDisplay(Name = "CompanyAdmin", ResourceType = typeof(ResourcesRest))]
        CompanyAdministrator = 3,
        [WrmDisplay(Name = "SystemAdmin", ResourceType = typeof(ResourcesRest))]
        SystemAdministrator = 4,
    }
}
