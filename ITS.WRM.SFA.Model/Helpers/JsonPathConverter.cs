using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class JsonPathConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType,
                                    object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            object targetObj = Activator.CreateInstance(objectType);

            foreach (PropertyInfo prop in objectType.GetRuntimeProperties()
                                                    .Where(p => p.CanRead && p.CanWrite))
            {
                JsonPropertyAttribute att = prop.GetCustomAttributes(true)
                                                .OfType<JsonPropertyAttribute>()
                                                .FirstOrDefault();

                string jsonPath = (att != null ? att.PropertyName : prop.Name);
                JToken token = jo.SelectToken(jsonPath);

                if (token != null && token.Type != JTokenType.Null)
                {
                    object value = token.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(targetObj, value, null);
                }
            }

            return targetObj;
        }

        public override bool CanConvert(Type objectType)
        {
            // CanConvert is not called when [JsonConverter] attribute is used
            return false;
        }

        //public override bool CanWrite
        //{
        //    get { return false; }
        //}

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            var properties = value.GetType().GetRuntimeProperties().Where(p => p.CanRead
                                                                            && p.CanWrite
                                                                            && p.GetCustomAttributes<JsonIgnoreAttribute>().Count() <= 0);

            JObject main = new JObject();
            List<string> propsNames = properties.Select(p => p.Name).ToList();
            foreach (PropertyInfo prop in properties)
            {
                try
                {
                    JsonPropertyAttribute att = prop.GetCustomAttributes(true)
                        .OfType<JsonPropertyAttribute>()
                        .FirstOrDefault();

                    string jsonPath = att != null && att.PropertyName != null ? att.PropertyName : prop.Name;


                    if (prop.PropertyType.FullName.Contains("System.Collections.Generic.List"))
                    {
                        jsonPath = jsonPath.Split('.').First();
                        var propertyValue = prop.GetValue(value);
                        if (propertyValue != null)
                        {
                            string listString = JsonConvert.SerializeObject(propertyValue);
                            main[jsonPath] = JArray.Parse(listString);
                        }
                        else
                        {
                            main[jsonPath] = new JArray();
                        }
                    }
                    else
                    {
                        ComplexSerialiseAttribute complexSerialiseAttribute = prop.GetCustomAttributes<ComplexSerialiseAttribute>().FirstOrDefault();
                        if (complexSerialiseAttribute != null)
                        {
                            jsonPath = jsonPath.Split('.').First();
                            PropertyInfo complexProperty = value.GetType().GetRuntimeProperty(complexSerialiseAttribute.SerialiseProperty);
                            var complexPropertyValue = complexProperty.GetValue(value);
                            if (complexPropertyValue is BasicObj)
                            {
                                string complexResult = JsonConvert.SerializeObject(complexPropertyValue);
                                main[jsonPath] = JObject.Parse(complexResult);
                            }
                            else
                            {
                                main[jsonPath] = JObject.Parse(complexPropertyValue == null ? "{}" : complexPropertyValue.ToString());
                            }
                        }
                        else
                        {
                            if (serializer.ContractResolver is DefaultContractResolver)
                            {
                                var resolver = (DefaultContractResolver)serializer.ContractResolver;
                                jsonPath = resolver.GetResolvedPropertyName(jsonPath);
                            }
                            var nesting = jsonPath.Split('.');
                            JObject lastLevel = main;
                            for (int i = 0; i < nesting.Length; i++)
                            {
                                if (i == nesting.Length - 1)
                                {
                                    var propertyValue = prop.GetValue(value);


                                    lastLevel[nesting[i]] = new JValue(propertyValue);

                                }
                                else
                                {
                                    if (lastLevel[nesting[i]] == null)
                                    {
                                        lastLevel[nesting[i]] = new JObject();
                                    }
                                    lastLevel = (JObject)lastLevel[nesting[i]];
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            serializer.Serialize(writer, main);
        }
        public class AmountModel
        {
            public string Amounts;
        }
    }
}
