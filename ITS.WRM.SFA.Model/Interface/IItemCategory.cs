using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IItemCategory: ICategoryNode
    {
        string FullDescription { get; set; }
        decimal Points { get; set; }
    }
}
