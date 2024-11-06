using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 180, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class MinistryDocumentType : LookupField
    {
        public string Code { get; set; }

        public eDocumentValueFactor DocumentValueFactor { get; set; }

        public bool IsSupported { get; set; }

        public string ShortTitle { get; set; }

        public string Title { get; set; }

        public bool Update { get; set; }

    }
}
