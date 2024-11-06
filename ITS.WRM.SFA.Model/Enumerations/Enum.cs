using ITS.WRM.SFA.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace ITS.WRM.SFA.Model.Enumerations
{
    public static class Enum<T> where T : struct
    {
        public static T Parse(string name)
        {
            return Parse(name, false);
        }
        public static T Parse(string name, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), name, ignoreCase);
        }
        public static T? TryParse(string name)
        {
            return TryParse(name, false);
        }
        public static T? TryParse(string name, bool ignoreCase)
        {
            T value;
            if (!string.IsNullOrEmpty(name) && TryParse(name, out value, ignoreCase))
                return value;
            return null;
        }
        public static bool TryParse(string name, out T value)
        {
            return TryParse(name, out value, false);
        }
        public static bool TryParse(string name, out T value, bool ignoreCase)
        {
            try
            {
                value = Parse(name, ignoreCase);
                return true;
            }
            catch (ArgumentException)
            {
                value = default(T);
                return false;
            }
        }
        public static IEnumerable<T> ToEnumerable()
        {
            foreach (var value in Enum.GetValues(typeof(T)))
                yield return (T)value;
        }
        public static IEnumerable<object> GetValues()
        {
            foreach (var value in Enum.GetValues(typeof(T)))
                yield return value;
        }
        public static IDictionary<object, string> GetDictionary()
        {
            Dictionary<object, string> dictionary = new Dictionary<object, string>();
            foreach (var value in GetValues())
                dictionary.Add(
                    Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T))),
                    GetName(value));
            return dictionary;
        }
        #region Strongly-Typed Enum Extenders
        public static string Format(object value, string format)
        {
            return Enum.Format(typeof(T), value, format);
        }
        public static string GetName(object value)
        {
            return Enum.GetName(typeof(T), value);
        }
        public static IEnumerable<string> GetNames()
        {
            return Enum.GetNames(typeof(T));
        }
        public static Type GetUnderlyingType()
        {
            return Enum.GetUnderlyingType(typeof(T));
        }
        public static bool IsDefined(object value)
        {
            return Enum.IsDefined(typeof(T), value);
        }
        public static T ToObject(object value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(byte value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(sbyte value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(uint value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(long value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(ulong value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(short value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
        public static T ToObject(ushort value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static IDictionary<T, string> GetLocalizedDictionary()
        {
            Dictionary<T, string> dictionary = new Dictionary<T, string>();
            foreach (var value in ToEnumerable())
                dictionary.Add(
                    value,
                    ToLocalizedString(value));
            return dictionary;
        }
        public static string ToLocalizedString(T value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    WrmDisplayAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(WrmDisplayAttribute)) as WrmDisplayAttribute;
                    if (attr != null)
                    {
                        Type resourceType = attr.ResourceType;
                        PropertyInfo resource = resourceType.GetProperty(attr.Name,
                                           BindingFlags.Static | BindingFlags.Public);
                        return resource.GetValue(null, null) as String;
                    }
                }
            }
            return value.ToString();
        }
#endregion
    }
}
