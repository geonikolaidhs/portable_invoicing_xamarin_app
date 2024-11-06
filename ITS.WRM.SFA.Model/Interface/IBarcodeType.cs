
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IBarcodeType : ILookUp2Fields, IRequiredOwner
    {
        bool NonSpecialCharactersIncluded { get; set; }
        bool PrefixIncluded { get; set; }
        bool IsWeighed { get; set; }
        string Prefix { get; set; }
        string Mask { get; set; }
        bool HasMixInformation { get; set; }
        string EntityType { get; set; }
    }
}
