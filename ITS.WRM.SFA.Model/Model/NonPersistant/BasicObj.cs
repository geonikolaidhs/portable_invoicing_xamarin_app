using ITS.WRM.SFA.Model.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ITS.WRM.SFA.Model.Model;

namespace ITS.WRM.SFA.Model.NonPersistant
{
    public class BasicObj
    {

        public BasicObj()
        {
            //this.CreatedOnTicks = this.UpdatedOnTicks = DateTime.Now.Ticks;

        }

        [PrimaryKey, AutoIncrement]
        [Indexed]
        public Guid Oid { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool RowDeleted { get; set; }
        public long UpdatedOnTicks { get; set; }
        public long CreatedOnTicks { get; set; }
        public string CreatedByDevice { get; set; }
        public string UpdateByDevice { get; set; }
        public string ExtraDescription { get; set; }
        public string ReferenceId { get; set; }

        [Ignore]
        public DateTime CreatedOn
        {
            get
            {
                return new DateTime(CreatedOnTicks);
            }
            set
            {
                if (value == null)
                {
                    CreatedOnTicks = DateTime.MinValue.Ticks;
                }
                else
                {
                    CreatedOnTicks = value.Ticks;
                }
            }

        }
        [Ignore]
        public DateTime UpdatedOn
        {
            get
            {
                return new DateTime(UpdatedOnTicks);
            }
            set
            {
                if (value == null)
                {
                    UpdatedOnTicks = DateTime.MinValue.Ticks;
                }
                else
                {
                    UpdatedOnTicks = value.Ticks;
                }
            }

        }

        public string GetExpandUrl()
        {
            string ExpandResult = "";
            try
            {
                List<string> propertyList = new List<string>();
                ExpandResult = "$expand=";

                bool IsIgnoreExpand = false;
                var type = this.GetType().GetTypeInfo().GetCustomAttributes<IgnoreExpand>();
                if (type.Any())
                {
                    IsIgnoreExpand = true;
                }
                foreach (var prop in this.GetType().GetRuntimeProperties().Where((typ => typ.GetCustomAttributes<ExpandPropertyAttribute>().Count() == 1 && IsIgnoreExpand == false)).ToList())
                {
                    propertyList.Add(prop.Name);
                    ExpandResult += string.Format("{0}($select=Oid),", prop.Name);
                }
                if (propertyList.Any())
                {
                    ExpandResult = ExpandResult.Remove(ExpandResult.Length - 1);
                    return ExpandResult;
                }
                else
                {
                    ExpandResult = "";
                }
            }
            catch (Exception ex)
            {
            }
            return ExpandResult;
        }
    }
}
