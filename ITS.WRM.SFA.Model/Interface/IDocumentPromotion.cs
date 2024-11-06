using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IDocumentPromotion: IBaseObj
    {
        IPromotion Promotion { get; set; }
        IDocumentHeader DocumentHeader { get; set; }
        string PromotionCode { get; set; }
        string PromotionDescription { get; set; }
        decimal TotalGain { get; set; }
        Guid DocumentHeaderOid { get; set; }
    }
}
