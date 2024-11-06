using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{

    [IgnoreExpand]
    [CreateOrUpdaterOrder(Order = 440, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class ItemCategory : CategoryNode
    {
        public string FullDescription { get; set; }
        public decimal Points { get; set; }

        [Ignore]
        [JsonIgnore]
        private List<Guid> _CategoryOids { get; set; } = null;

        [Ignore]
        [JsonIgnore]
        public List<Guid> CategoryOids
        {
            get
            {
                if (_CategoryOids == null)
                {
                    _CategoryOids = new List<Guid>();
                    if (string.IsNullOrEmpty(OidString))
                    {
                        _CategoryOids.Add(this.Oid);
                    }
                    else
                    {
                        string[] split = OidString.Split(',');
                        if (split != null && split.Length > 0)
                            for (int i = 0; i < split.Length; i++)
                            {
                                Guid id;
                                if (Guid.TryParse(split[i], out id))
                                    _CategoryOids.Add(new Guid());
                            }
                        if (!_CategoryOids.Contains(this.Oid))
                            _CategoryOids.Add(this.Oid);
                    }
                    return _CategoryOids;
                }
                else
                {
                    return _CategoryOids;
                }
            }
        }


        [Ignore]
        [JsonIgnore]
        private String OidString { get; set; }

    }
}
