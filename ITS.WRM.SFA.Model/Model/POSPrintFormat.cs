using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 19, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class POSPrintFormat : LookupField
    {
        public string Format { get; set; }

        public eFormatType FormatType { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public DocumentType DocumentType { get; set; }

        [ForeignKey(typeof(DocumentType))]
        [Column("DocumentType")]
        [JsonProperty(PropertyName = "DocumentType.Oid")]
        public Guid DocumentTypeOid { get; set; }
    }
}
