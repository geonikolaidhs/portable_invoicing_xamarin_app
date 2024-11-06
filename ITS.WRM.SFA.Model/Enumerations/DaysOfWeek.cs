using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [Flags]
    public enum DaysOfWeek
    {
        [WrmDisplay(Name = "NoneDay", ResourceType = typeof(ResourcesRest))]
        None = 0 ,
        [WrmDisplay(Name = "Sunday", ResourceType = typeof(ResourcesRest))]
        Sunday = 1,
        [WrmDisplay(Name = "Monday", ResourceType = typeof(ResourcesRest))]
        Monday = 2,
        [WrmDisplay(Name = "Tuesday", ResourceType = typeof(ResourcesRest))]
        Tuesday = 4 ,
        [WrmDisplay(Name = "Wednesday", ResourceType = typeof(ResourcesRest))]
        Wednesday = 8 ,
        [WrmDisplay(Name = "Thursday", ResourceType = typeof(ResourcesRest))]
        Thursday = 16,
        [WrmDisplay(Name = "Friday", ResourceType = typeof(ResourcesRest))]
        Friday = 32,
        [WrmDisplay(Name = "Saturday", ResourceType = typeof(ResourcesRest))]
        Saturday = 64,
        
    }
}
