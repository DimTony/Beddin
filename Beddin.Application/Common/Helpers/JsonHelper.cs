using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Beddin.Application.Common.Helpers
{
    public class JsonHelper
    {
        public static JsonSerializerSettings GetJsonSerializer
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(), true));
                return settings;
            }
        }

        public static bool IsValidJson(string value)
        {
            try
            {
                _ = JToken.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumeric(Type? type)
        {
            if (type is null) return false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];

            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or TypeCode.SByte or
                TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or
                TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or
                TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false
            };
        }
        public static T MaskValues<T>(T payload)
        {
            // MemberwiseClone always exists — safe to use null-forgiving here
            var method = payload!.GetType()
                .GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;

            var tempData = (T)method.Invoke(payload, null)!;

            var sensitiveFields = new[] { "Password", "TransactionPin", "Pin", "OldTransactionPin", "NewTransactionPin" };

            var matchedProperties = tempData!.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => sensitiveFields.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
                .ToList();

            foreach (var property in matchedProperties)
            {
                // Only set if the property type accepts string (guard against non-string sensitive props)
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(object))
                    property.SetValue(tempData, "********");
            }

            return tempData;
        }

        public static JObject MaskValues(JObject payload)
        {
            var sensitiveFields = new[] { "password", "transactionPin", "pin", "oldTransactionPin", "newTransactionPin" };

            foreach (var field in sensitiveFields)
            {
                if (payload.ContainsKey(field))
                    payload[field] = "********";
            }

            return payload;
        }

        public static JsonSerializerSettings IgnoreOrRenameJsonProperties<T>(
            string[]? ignoreList = null,
            Dictionary<string, string>? renameList = null) where T : class
        {
            var jsonResolver = new JsonPropertyRenameAndIgnoreSerializerContractResolver();

            if (ignoreList is not null)
            {
                foreach (var property in ignoreList)
                    jsonResolver.IgnoreProperty(typeof(T), property);
            }

            if (renameList is not null)
            {
                foreach (var (key, value) in renameList)
                    jsonResolver.RenameProperty(typeof(T), key, value);
            }

            return new JsonSerializerSettings { ContractResolver = jsonResolver };
        }
        public static string CleanJson(string json, string[] properties)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            var token = JToken.Parse(json);

            foreach (var property in properties)
            {
                foreach (var field in token.SelectTokens(property).ToList())
                {
                    if (field is not null && field.Type != JTokenType.String)
                        field.Replace(JValue.CreateNull()); // was: JValue jvalue = null; field.Replace(jvalue)
                }
            }

            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }

        public static string ObjectToArray(string json, string[] properties)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            var token = JToken.Parse(json);

            foreach (var property in properties)
            {
                foreach (var field in token.SelectTokens(property).ToList())
                {
                    if (field is not null && field.Type != JTokenType.Array)
                    {
                        var array = new JArray { field.ToObject<object>()! };
                        field.Replace(array);
                    }
                }
            }

            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }
    }

    public class JsonPropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores = new();
        private readonly Dictionary<Type, Dictionary<string, string>> _renames = new();

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
                _ignores[type] = new HashSet<string>();

            foreach (var prop in jsonPropertyNames)
                _ignores[type].Add(prop);
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
                _renames[type] = new Dictionary<string, string>();

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType is not null && property.PropertyName is not null)
            {
                if (IsIgnored(property.DeclaringType, property.PropertyName))
                {
                    property.ShouldSerialize = _ => false;
                    property.Ignored = true;
                }

                if (IsRenamed(property.DeclaringType, property.PropertyName, out var newName))
                    property.PropertyName = newName;
            }

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            return _ignores.TryGetValue(type, out var set) && set.Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string? newJsonPropertyName)
        {
            if (_renames.TryGetValue(type, out var renames) && renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
                return true;

            newJsonPropertyName = null;
            return false;
        }
    }
}