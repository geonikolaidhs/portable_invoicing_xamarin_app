using ITS.WRM.SFA.Model.Attributes;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class CustomFieldStorage : BaseObj
    {
        public DateTime DateField5 { get; set; }
        public DateTime DateField4 { get; set; }
        public DateTime DateField3 { get; set; }
        public DateTime DateField2 { get; set; }
        public DateTime DateField1 { get; set; }
        public decimal DecimalField5 { get; set; }
        public decimal DecimalField4 { get; set; }
        public decimal DecimalField3 { get; set; }
        public decimal DecimalField2 { get; set; }
        public decimal DecimalField1 { get; set; }
        public string StringField5 { get; set; }
        public string StringField4 { get; set; }
        public string StringField3 { get; set; }
        public string StringField2 { get; set; }
        public string StringField1 { get; set; }
        public int IntegerField5 { get; set; }
        public int IntegerField4 { get; set; }
        public int IntegerField3 { get; set; }
        public int IntegerField2 { get; set; }
        public int IntegerField1 { get; set; }


        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public CustomEnumerationValue CustomEnumerationValue1 { get; set; }

        [ForeignKey(typeof(CustomEnumerationValue))]
        [Column("CustomEnumerationValue1")]
        [JsonProperty("CustomEnumerationValue1.Oid")]
        public Guid CustomEnumerationValue1Oid { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        [Ignore]
        [ExpandProperty]
        [JsonIgnore]
        public CustomEnumerationValue CustomEnumerationValue2 { get; set; }

        [ForeignKey(typeof(CustomEnumerationValue))]
        [Column("CustomEnumerationValue2")]
        [JsonProperty("CustomEnumerationValue2.Oid")]
        public Guid CustomEnumerationValue1Oid2 { get; set; }

        [ForeignKey(typeof(CustomEnumerationValue))]
        [Column("CustomEnumerationValue3")]
        [JsonProperty("CustomEnumerationValue3.Oid")]
        public Guid CustomEnumerationValue1Oid3 { get; set; }

        [ForeignKey(typeof(CustomEnumerationValue))]
        [Column("CustomEnumerationValue4")]
        [JsonProperty("CustomEnumerationValue4.Oid")]
        public Guid CustomEnumerationValue1Oid4 { get; set; }

        [ForeignKey(typeof(CustomEnumerationValue))]
        [Column("CustomEnumerationValue5")]
        [JsonProperty("CustomEnumerationValue5.Oid")]
        public Guid CustomEnumerationValue1Oid5 { get; set; }


    }
}
