using ITS.WRM.SFA.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface ICustomReport : ILookUpFields
    {
        string Code { get; set; }
        string Title { get; set; }
        byte[] ReportFil { get; set; }
        string FileName { get; set; }
        eCultureInfo CultureInfo { get; set; }
        ICompanyNew Owner { get; set; }
        //IReportCategory ReportCategory { get; set; }
        string ObjectType { get; set; }
        string ReportType { get; set; }
    }
}
