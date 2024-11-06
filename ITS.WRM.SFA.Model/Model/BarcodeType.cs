
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using SQLiteNetExtensions.Attributes;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Interface;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 260, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class BarcodeType : LookUp2Fields, IRequiredOwner
    {

        public string EntityType { get; set; }

        public bool HasMixInformation { get; set; }

        public bool IsWeighed { get; set; }

        public string Mask { get; set; }

        public bool NonSpecialCharactersIncluded { get; set; }

        public string Prefix { get; set; }

        public bool PrefixIncluded { get; set; }

        public int Length
        {
            get
            {
                int prefixLength = !string.IsNullOrEmpty(Prefix) ? Prefix.Length : 0;
                int maskLength = !string.IsNullOrEmpty(Mask) ? Mask.Length : 0;
                return prefixLength + maskLength;
            }
        }

        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }

    }
}
