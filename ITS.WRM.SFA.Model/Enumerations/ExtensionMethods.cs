using ITS.WRM.SFA.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ITS.WRM.SFA.Model.Enumerations
{

    public class EntityDisplayNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type ResourceType { get; private set; }
        private WrmDisplayAttribute display;

        public EntityDisplayNameAttribute(string name, Type resourceType)
        {
            this.Name = name;
            this.ResourceType = resourceType;
            display = new WrmDisplayAttribute() { Name = name, ResourceType = resourceType };

        }


    }

    public static class ExtensionMethods
    {
        public static Dictionary<T, string> ToDictionary<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            T obj = Activator.CreateInstance<T>();

            //IEnumerable<String> names = Enum.GetNames(typeof(T));
            IEnumerable<T> values = Enum.GetValues(typeof(T)).Cast<T>();
            return values.ToDictionary(x => x, x => Enum<T>.ToLocalizedString(x));
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    WrmDescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(WrmDescriptionAttribute)) as WrmDescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return value.ToString();
        }

        public static string ToLocalizedString(this Type type)
        {
            EntityDisplayNameAttribute attr = type.GetCustomAttributes(typeof(EntityDisplayNameAttribute), false).FirstOrDefault() as EntityDisplayNameAttribute;
            if (attr != null && !String.IsNullOrWhiteSpace(attr.Name))
            {
                Type resourceType = attr.ResourceType;
                PropertyInfo resource = resourceType.GetProperty(attr.Name,
                                   BindingFlags.Static | BindingFlags.Public);
                if (resource != null)
                {
                    return resource.GetValue(null, null) as String;
                }
                else
                {
                    return attr.Name;
                }
            }

            return type.Name;
        }

        public static string ToLocalizedString(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (string.IsNullOrEmpty(name) == false)//if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    WrmDisplayAttribute attr = (WrmDisplayAttribute)Attribute.GetCustomAttribute(field, typeof(WrmDisplayAttribute));
                    if (attr != null && !String.IsNullOrWhiteSpace(attr.Name))
                    {
                        Type resourceType = attr.ResourceType;
                        PropertyInfo resource = resourceType.GetProperty(attr.Name, BindingFlags.Static | BindingFlags.Public);
                        if (resource != null)
                        {
                            return resource.GetValue(null, null) as String;
                        }
                        else
                        {
                            return attr.Name;
                        }
                    }
                }
            }
            return value.ToString();
        }





        public static eDocumentTraderType GetAvailableTraderTypes(this eDivision division)
        {
            FieldInfo field = typeof(eDivision).GetField(division.ToString());
            if (field != null)
            {
                AvailabeTraderTypeAttribute[] attributes =
                       Attribute.GetCustomAttributes(field,
                         typeof(AvailabeTraderTypeAttribute)) as AvailabeTraderTypeAttribute[];
                if (attributes != null && attributes.Length == 1)
                {
                    return attributes[0].AvailableTraderTypes;
                }
            }
            return eDocumentTraderType.NONE;
        }


        public static IEnumerable<T> ToEnumValuesEnumerable<T>(this T mask) where T : struct, IComparable, IFormattable, IConvertible
        {
            if (mask is Enum == false)
            {
                throw new ArgumentException();
            }
            Enum maskEnum = mask as Enum;
            return Enum.GetValues(typeof(T))
                                 .Cast<Enum>()
                                 .Where(val => maskEnum.HasFlag(val) && Convert.ToInt32(val) != 0)
                                 .Cast<T>();
        }


    }
}
