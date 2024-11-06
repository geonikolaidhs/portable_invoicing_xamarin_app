using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AvailabeTraderTypeAttribute : Attribute
    {
        public eDocumentTraderType AvailableTraderTypes { get; private set; }
        public AvailabeTraderTypeAttribute(eDocumentTraderType availableTraderTypes)
        {
            this.AvailableTraderTypes = availableTraderTypes;
        }
    }
}
