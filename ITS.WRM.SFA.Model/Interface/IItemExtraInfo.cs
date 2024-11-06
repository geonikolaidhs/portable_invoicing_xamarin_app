using ITS.WRM.SFA.Model.Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IItemExtraInfo: IBasicObj, IRequiredOwner
    {
        IItem Item { get; set; }
        string Ingredients { get; set; }
        DateTime PackedAt { get; set; }
        DateTime ExpiresAt { get; set; }

    }
}
