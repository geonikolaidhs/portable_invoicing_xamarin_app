using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITS.WRM.SFA.Model.Model.NonPersistant;
using SQLite;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using Newtonsoft.Json;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 150, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentStatus : LookUp2Fields, IRequiredOwner
    {
        [OneToMany(CascadeOperations = CascadeOperation.All), JsonIgnore]
        public List<DocumentHeader> DocumentHeaders { get; set; }
        
        public bool ReadOnly { get; set; }
        
        public bool TakeSequence { get; set; }
        [Ignore]
        string IOwner.Description { get;  }

        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }
    }
}
