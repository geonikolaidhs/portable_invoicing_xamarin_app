using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLite;
using SQLiteNetExtensions.Attributes;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Interface;

namespace ITS.WRM.SFA.Model.Model
{
    public class DeficiencySettings : LookUp2Fields, IRequiredOwner
    {

        public DocumentType DeficiencyDocumentType { get; set; }
        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }
    }
}
