using ITS.WRM.SFA.Model.Interface;
using System;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;

using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using SQLite;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 230, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentSeries : LookUp2Fields, IRequiredOwner
    {

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(DocumentSequence))]
        public DocumentSequence DocumentSequence { get; set; }

        [ForeignKey(typeof(DocumentSequence))]
        [Column("DocumentSequence")]
        [JsonProperty(PropertyName = "DocumentSequence.Oid")]
        public Guid DocumentSequenceOid { get; set; }


        public eModule eModule { get; set; }

        public bool HasAutomaticNumbering { get; set; }

        public Guid? IsCanceledBy { get; set; }

        public bool IsCancelingSeries { get; set; }

        public Guid POS { get; set; }

        public string PrintedCode { get; set; }

        public string Remarks { get; set; }
        public bool ShouldResetMenu { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [JsonIgnore]
        [ExpandProperty]
        public Store Store { get; set; }

        [ForeignKey(typeof(Store))]
        [Column("Store")]
        [JsonProperty(PropertyName = "Store.Oid")]
        public Guid StoreOid { get; set; }


        ICompanyNew IOwner.Owner
        {
            get
            {
                return this.Owner;
            }
        }

        public string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(Remarks))
                {
                    return Description;
                }
                return String.Format("{0}({1}", Description, Remarks) + ")";
            }
        }

    }
}
