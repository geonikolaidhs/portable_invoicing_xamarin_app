using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IUser : IBaseObj, IUserModel, IPersistentObjectPortable
    {
        IDocumentHeader WishList { get; set; }
        bool IsB2CUser { get; set; }
        DateTime LastLoginDate { get; set; }
        string Comment { get; set; }
        DateTime FailedPasswordAttemptWindowStart { get; set; }
        int FailedPasswordAttemptCount { get; set; }
        DateTime LastLockedOutDate { get; set; }
        DateTime LastActivityDate { get; set; }
        bool IsLockedOut { get; set; }
        DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        int FailedPasswordAnswerAttemptCount { get; set; }
        string PasswordAnswer { get; set; }
        string PasswordQuestion { get; set; }
        bool IsApproved { get; set; }
        string Email { get; set; }
        string Key { get; set; }
        IStore CentralStore { get; set; }
        bool IsCentralStore { get; set; }
        bool TermsAccepted { get; set; }
        //string UserName { get; set; }
        string FullName { get; set; }
        //string Password { get; set; }
        string TaxCode { get; set; }
        IRole Role { get; set; }
        string POSUserName { get; set; }
        string POSPassword { get; set; }
        ePOSUserLevel POSUserLevel { get; set; }
        string AuthToken { get; set; }
    }
}
