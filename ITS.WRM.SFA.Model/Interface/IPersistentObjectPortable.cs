using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IPersistentObjectPortable
    {
        Guid Oid { get; }

        DateTime CreatedOn { get; }

        long CreatedOnTicks { get; }

        DateTime UpdatedOn { get; }

        long UpdatedOnTicks { get; }

        IUserPortable CreatedBy { get; }

        IUserPortable UpdatedBy { get; }

        string CreatedByDevice { get; }

        string UpdateByDevice { get; }

        bool RowDeleted { get; }

        bool IsActive { get; }

        bool IsSynchronized { get; }

        string MLValues { get; }

        Guid MasterObjOid { get; }

        string ReferenceId { get; }

        string ObjectSignature { get; }
    }
}
