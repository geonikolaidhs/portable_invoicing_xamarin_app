using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class DeviceTypeAttribute : Attribute
    {
        public DeviceType DeviceType { get; protected set; }

        public DeviceTypeAttribute(DeviceType deviceType)
        {
            DeviceType = deviceType;
        }
    }
}
