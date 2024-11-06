using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComplexSerialiseAttribute : Attribute
    {
        public string SerialiseProperty { get; set; }
    }
}
