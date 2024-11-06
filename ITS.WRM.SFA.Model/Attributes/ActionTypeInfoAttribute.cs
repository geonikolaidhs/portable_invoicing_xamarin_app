using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.WRM.SFA.Model.Attributes
{
    /// <summary>
    /// An Attribute describing where the ActionType will be applied and how
    /// </summary>
    public class ActionTypeInfoAttribute : Attribute
    {
        /// <summary>
        /// The Master Entity that the ActionType will be applied
        /// </summary>
        public string MasterEntity { get; set; }

        /// <summary>
        /// The 'path' to the Property of the MasterEntity. For each of these property (if many) a VariableValues will be calculated.
        /// Seperate path with '.' and make sure to Comply with object properties.
        /// If the ActionType should execute once per MasterEntity leave blank.
        /// </summary>
        public string DetailEntity { get; set; }

        /// <summary>
        /// Defines if Store filter will be used on Detail entity
        /// </summary>
        public string BasicObjCriteria { get; set; }

        public string Criteria { get; set; }

        public ActionTypeInfoAttribute(string masterEntity, string detailEntity = "", string criteriaString = "", string basicObjCriteria = "")
        {
            this.MasterEntity = masterEntity;
            this.DetailEntity = detailEntity;
            if (string.IsNullOrEmpty(basicObjCriteria) == false)
            {
                this.BasicObjCriteria = basicObjCriteria;
            }
            if (string.IsNullOrEmpty(criteriaString) == false)
            {
                this.Criteria = criteriaString;
            }
        }

        public List<string> Path
        {
            get
            {
                if (string.IsNullOrEmpty(this.DetailEntity))
                {
                    return new List<string>();
                }
                return this.DetailEntity.Split('.').ToList();
            }
        }
    }
}
