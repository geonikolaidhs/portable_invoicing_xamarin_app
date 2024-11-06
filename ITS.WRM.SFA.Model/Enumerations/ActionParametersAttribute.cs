using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
    public class ActionKeybindParameterAttribute : Attribute
    {
        public string Parameter { get; set; }
        public Type ParameterType { get; set; }

        public ActionKeybindParameterAttribute(string parameterName,Type parameterType)
        {
            this.Parameter = parameterName;
            this.ParameterType = parameterType;
        }

    }
}
