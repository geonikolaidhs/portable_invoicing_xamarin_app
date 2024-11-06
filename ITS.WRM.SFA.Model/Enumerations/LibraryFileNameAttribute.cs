using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class LibraryFilenameAttribute : Attribute
    {
        public string LibraryFilename { get; set; }

        public LibraryFilenameAttribute(string libraryFilename)
        {
            LibraryFilename = libraryFilename;
        }

    }
}
