using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    [CreateOrUpdaterOrder(Order = 240, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    [JsonConverter(typeof(JsonPathConverter))]
    public class StoreDocumentSeriesType : BasicObj
    {
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(DocumentSeries))]
        public DocumentSeries DocumentSeries { get; set; }

        [ForeignKey(typeof(DocumentSeries))]
        [Column("DocumentSeries")]
        [JsonProperty("DocumentSeries.Oid")]
        public Guid DocumentSeriesOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(Customer))]
        public Customer DefaultCustomer { get; set; }

        [ForeignKey(typeof(Customer))]
        [Column("DefaultCustomer")]
        [JsonProperty("DefaultCustomer.Oid")]
        public Guid DefaultCustomerOid { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(Supplier))]
        public Supplier DefaultSupplier { get; set; }

        [ForeignKey(typeof(Supplier))]
        [Column("DefaultSupplier")]
        [JsonProperty("DefaultSupplier.Oid")]
        public Guid DefaultSupplierOid { get; set; }

                
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        [ExpandProperty]
        [ForeignKey(typeof(DocumentType))]
        public DocumentType DocumentType { get; set; }

        [ForeignKey(typeof(DocumentType))]
        [Column("DocumentType")]
        [JsonProperty("DocumentType.Oid")]
        public Guid DocumentTypeOid { get; set; }


        //public CustomReport _DefaultCustomReport;
        public int Duplicates;

        public UserType UserType;

        public eStoreDocumentType StoreDocumentType;

        public string MenuDescription;


        public string Description
        {
            get
            {
                string documentSeriesFullName = DocumentSeries == null ? "" : DocumentSeries.FullName;
                string description = String.IsNullOrWhiteSpace(MenuDescription) ?
                                     String.Format("{0} {1}", DocumentType.Description, documentSeriesFullName) :
                                     MenuDescription;
                return description;
            }
        }
    }
}
