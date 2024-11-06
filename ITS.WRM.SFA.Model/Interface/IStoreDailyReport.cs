using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  ITS.WRM.SFA.Model.Interface
{
    public interface IStoreDailyReport: IBaseObj
    {
        IStore Store { get; set;  }
        decimal CollectionsTotal { get; set; }
        decimal MainPOSWithdraws { get; set; }
        decimal Coins { get; set; }
        decimal PaperMoney { get; set; }
        decimal OtherExpanses { get; set; }
        decimal DailyTotalsTotal { get; set; }
        decimal InvoicesTotal { get; set; }
        decimal AutoDeliveriesTotal { get; set; }
        decimal PaymentsTotal { get; set; }
        decimal PaymentsWithDrawsTotal { get; set; }
        decimal CreditsTotal { get; set; }
        decimal CreditsPaymentsWithDrawsTotal { get; set; }
        decimal CashDelivery { get; set; }
        decimal InvoicesTotalCash { get; set; }
        decimal ReportTotal { get; set; }
        decimal POSDifference { get; set;  }
        int? Code { get; set; }
        DateTime ReportDate { get; set; }
        decimal CreditsGridTotal { get; set; }
        ICompanyNew Owner { get; set; }
        decimal CollectionComplement { get; set; }
        string CollectionComplementText { get; set; }
        string OtherExpansesText { get; set; }
    }
}
