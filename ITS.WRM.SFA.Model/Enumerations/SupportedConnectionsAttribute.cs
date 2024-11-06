using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class SupportedConnectionsAttribute : Attribute
    {
        public ConnectionType[] SupportedConnections { get; protected set; }

        public SupportedConnectionsAttribute(params ConnectionType[] supportedConnections)
        {
            SupportedConnections = supportedConnections;
        }

    }

}
