using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 2, Permissions = eUpdateDirection.SFA_TO_MASTER)]
    public class TableVersion
    {
        [PrimaryKey]
        public Guid Oid { get; set; }
        public string TableName { get; set; }

        public long UpdatedOnticks { get; set; }

        public long Version { get; set; }


    }
}
