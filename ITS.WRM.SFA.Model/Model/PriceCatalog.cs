using ITS.WRM.SFA.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.Attributes;
using SQLiteNetExtensions.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using Newtonsoft.Json;
using ITS.WRM.SFA.Model.Helpers;

namespace ITS.WRM.SFA.Model.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    [CreateOrUpdaterOrder(Order = 53, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalog : LookUp2Fields
    {
        [Ignore]
        public DateTime EndDate
        {
            get { return new DateTime(lngEndDate); }
            set
            {
                if (value == null)
                {
                    lngEndDate = DateTime.MinValue.Ticks;
                }
                else
                {
                    lngEndDate = value.Ticks;
                }
            }
        }
        [Column("EndDate")]
        [JsonIgnore]
        public long lngEndDate { get; set; }
        [Ignore]
        public DateTime StartDate
        {
            get
            {
                return new DateTime(lngStartDate);
            }
            set
            {
                if (value == null)
                {
                    lngStartDate = DateTime.MinValue.Ticks;
                }
                else
                {
                    lngStartDate = value.Ticks;
                }
            }
        }
        [Column("StartDate")]
        [JsonIgnore]
        public long lngStartDate { get; set; }

        public bool IgnoreZeroPrices { get; set; }

        public Guid IsEditableAtStore { get; set; }

        public void SetDefaultEdit(Store EditableAtStore)
        {
            if (EditableAtStore == null)
            {
                this.IsEditableAtStore = Guid.Empty;
            }
            else
            {
                this.IsEditableAtStore = EditableAtStore.Oid;
            }
        }

        public bool IsRoot { get; set; }

        public int Level { get; set; }
        [Indexed]
        public Guid? ParentCatalogOid { get; set; }

        public void SetParentPriceCatalog(PriceCatalog parentPriceCatalog)
        {
            if (parentPriceCatalog == null)
            {
                this.ParentCatalogOid = Guid.Empty;
            }
            else
            {
                this.ParentCatalogOid = parentPriceCatalog.Oid;
            }
        }

        public PriceCatalog GetParentPriceCatalog(DatabaseLayer databaseLayer)
        {
            try
            {
                if (this.ParentCatalogOid.HasValue == false || this.ParentCatalogOid.Value == Guid.Empty)
                {
                    return null;
                }
                else
                {
                    return databaseLayer.GetStorePriceCatalogById(this.ParentCatalogOid.Value);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SupportLoyalty { get; set; }

    }
}
