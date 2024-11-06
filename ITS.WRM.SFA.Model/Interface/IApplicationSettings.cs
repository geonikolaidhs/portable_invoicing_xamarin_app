using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ITS.WRM.SFA.Model.Model.NonPersistant
{
    public interface IApplicationSettings : IBaseObj
    {
        int MainImageWidth { get; set; }
        int MainImageHeight { get; set; }
        //LogLevel LogingLevel { get; set; }
        int BigImageSizeMax { get; set; }
        int SmallImageSizeMax { get; set; }
        int BigImageHeight { get; set; }
        int BigImageWidth { get; set; }
        int SmallImageHeight { get; set; }
        int SmallImageWidth { get; set; }
        //Image MenuLogo { get; set; }
        //Image MainScreenImage { get; set; }
    }
}
