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
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 910, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentSequence : LookupField, IOwner
    {

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public DocumentSeries DocumentSeries { get; set; }

        [ForeignKey(typeof(DocumentSeries))]
        [Column("DocumentSeries")]
        [JsonProperty(PropertyName = "DocumentSeries.Oid")]
        public Guid DocumentSeriesOid { get; set; }

        //public string Description { get; set; }
        
        public int DocumentNumber { get; set; }
       
        public bool Update { get; set; }

        [Ignore]
        string IOwner.Description
        {
            get
            {
                return this.Description;
            }
        }
        [Ignore]
        ICompanyNew IOwner.Owner
        {
            get;
            
        }
    }
}
