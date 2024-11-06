using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IUserModel
    {
        string UserName { get; }
        string Password { get; }

        //IRole Role { get; }
    }
}
