using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Attributes
{
    public class WrmDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public WrmDescriptionAttribute() : this(string.Empty)
        {

        }

        public WrmDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
