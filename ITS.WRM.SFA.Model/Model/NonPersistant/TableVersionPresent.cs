using ITS.WRM.SFA.Model.Enumerations;
using System;

namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public class TableVersionPresent
    {
        public Guid Oid { get; set; }
        public string TableName { get; set; }
        public int Order { get; set; }
        public long UpdatedOnticks { get; set; }
        public long Version { get; set; }
        public long ServerVersion { get; set; }
        public DateTime ServerVersionDate
        {
            get
            {
                return new DateTime(ServerVersion);
            }
        }
        public DateTime VersionDate
        {
            get
            {
                return new DateTime(Version);
            }
        }
        public DateTime LastUpDate
        {
            get
            {
                return new DateTime(UpdatedOnticks);
            }
        }
    }
}
