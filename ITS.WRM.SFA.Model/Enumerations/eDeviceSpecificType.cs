using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Enumerations
{
    //για Custom υλοποιήσεις συσκευών
    public enum eDeviceSpecificType
    {
        None,
        [WrmDescription("Posiflex.PosiflexPD2x00")]
        [DeviceType(DeviceType.PoleDisplay)]
        [LibraryFilename("ITS.POS.Hardware.Posiflex.dll")]
        Posiflex_PosiflexPD2x00
    }



    public static class eDeviceSpecificTypeExtensions
    {
        public static DeviceType GetDeviceType(this eDeviceSpecificType deviceSpecificType)
        {
            try
            {
                Type type = typeof(eDeviceSpecificType);
                MemberInfo memInfo = type.GetMember(deviceSpecificType.ToString())[0];
                object[] attributes = memInfo.GetCustomAttributes(typeof(DeviceTypeAttribute), false);
                return ((DeviceTypeAttribute)attributes[0]).DeviceType;
            }
            catch
            {
                return DeviceType.Undefined;
            }
        }

        public static string GetLibraryFilename(this eDeviceSpecificType deviceSpecificType)
        {
            try
            {
                Type type = typeof(eDeviceSpecificType);
                MemberInfo memInfo = type.GetMember(deviceSpecificType.ToString())[0];
                object[] attributes = memInfo.GetCustomAttributes(typeof(LibraryFilenameAttribute), false);
                return ((LibraryFilenameAttribute)attributes[0]).LibraryFilename;
            }
            catch
            {
                return null;
            }
        }
    }
}
