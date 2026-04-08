// <copyright file="JsonHelper.cs" company="Beddin">
// Copyright (c) Beddin. All rights reserved.
// </copyright>

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Beddin.Application.Common.Helpers
{
    /// <summary>
    /// Provides helper methods for JSON serialization, deserialization, and manipulation.
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Gets the default <see cref="JsonSerializerSettings"/> with camel case property names,
        /// reference loop handling ignored, null value handling ignored, and string enum conversion.
        /// </summary>
        public static JsonSerializerSettings GetJsonSerializer
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                };
                settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(), true));
                return settings;
            }
        }

        /// <summary>
        /// Determines whether the specified string is a valid JSON.
        /// </summary>
        /// <param name="value">The string to validate as JSON.</param>
        /// <returns><c>true</c> if the string is valid JSON; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Determines whether the specified <see cref="Type"/> is a numeric type.
        /// </summary>
        /// <param name="type">The type to check for numeric characteristics.</param>
        /// <returns><c>true</c> if the type is numeric; otherwise, <c>false</c>.</returns>
        public static bool IsNumeric(Type? type)
        {
            if (type is null)
            {
                return false;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or TypeCode.SByte or
                TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or
                TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or
                TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }

        /// <summary>
        /// Returns a copy of the payload with sensitive values masked.
        /// </summary>
        /// <typeparam name="T">The type of the payload object.</typeparam>
        /// <param name="payload">The object whose sensitive values will be masked.</param>
        /// <returns>A copy of the payload with sensitive fields replaced by masked values.</returns>
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
                {
                    property.SetValue(tempData, "********");
                }
            }

            return tempData;
        }

        /// <summary>
        /// Returns a copy of the <see cref="JObject"/> payload with sensitive values masked.
        /// </summary>
        /// <param name="payload">The <see cref="JObject"/> whose sensitive values will be masked.</param>
        /// <returns>
        /// A <see cref="JObject"/> with sensitive fields replaced by masked values.
        /// </returns>
        public static JObject MaskValues(JObject payload)
        {
            var sensitiveFields = new[] { "password", "transactionPin", "pin", "oldTransactionPin", "newTransactionPin" };

            foreach (var field in sensitiveFields)
            {
                if (payload.ContainsKey(field))
                {
                    payload[field] = "********";
                }
            }

            return payload;
        }

        /// <summary>
        /// Creates a <see cref="JsonSerializerSettings"/> that allows ignoring or renaming JSON properties for the specified type.
        /// </summary>
        /// <typeparam name="T">The type for which to ignore or rename properties.</typeparam>
        /// <param name="ignoreList">An optional array of property names to ignore during serialization.</param>
        /// <param name="renameList">An optional dictionary mapping original property names to new JSON property names.</param>
        /// <returns>A <see cref="JsonSerializerSettings"/> configured to ignore or rename the specified properties.</returns>
        public static JsonSerializerSettings IgnoreOrRenameJsonProperties<T>(
            string[]? ignoreList = null,
            Dictionary<string, string>? renameList = null)
            where T : class
        {
            var jsonResolver = new JsonPropertyRenameAndIgnoreSerializerContractResolver();

            if (ignoreList is not null)
            {
                foreach (var property in ignoreList)
                {
                    jsonResolver.IgnoreProperty(typeof(T), property);
                }
            }

            if (renameList is not null)
            {
                foreach (var (key, value) in renameList)
                    jsonResolver.RenameProperty(typeof(T), key, value);
            }

            return new JsonSerializerSettings { ContractResolver = jsonResolver };
        }

        /// <summary>
        /// Cleans the specified JSON string by setting the specified properties to null if they are not strings.
        /// </summary>
        /// <param name="json">The JSON string to clean.</param>
        /// <param name="properties">An array of JSON property paths to clean.</param>
        /// <returns>The cleaned JSON string with specified properties set to null if not strings.</returns>
        public static string CleanJson(string json, string[] properties)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }

            var token = JToken.Parse(json);

            foreach (var property in properties)
            {
                foreach (var field in token.SelectTokens(property).ToList())
                {
                    if (field is not null && field.Type != JTokenType.String)
                    {
                        field.Replace(JValue.CreateNull()); // was: JValue jvalue = null; field.Replace(jvalue)
                    }
                }
            }

            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }

        /// <summary>
        /// Converts the specified properties in a JSON string to arrays if they are not already arrays.
        /// </summary>
        /// <param name="json">The JSON string to process.</param>
        /// <param name="properties">An array of JSON property paths to convert to arrays.</param>
        /// <returns>The JSON string with specified properties converted to arrays.</returns>
        public static string ObjectToArray(string json, string[] properties)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }

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

        /// <summary>
        /// A contract resolver that allows ignoring or renaming JSON properties for specific types during serialization.
        /// </summary>
        public class JsonPropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
        {
            private readonly Dictionary<Type, HashSet<string>> ignores = new();
            private readonly Dictionary<Type, Dictionary<string, string>> renames = new();

            /// <summary>
            /// Specifies properties to ignore for a given type during serialization.
            /// </summary>
            /// <param name="type">The type for which to ignore properties.</param>
            /// <param name="jsonPropertyNames">The property names to ignore.</param>
            public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
            {
                if (!this.ignores.ContainsKey(type))
                {
                    this.ignores[type] = new HashSet<string>();
                }

                foreach (var prop in jsonPropertyNames)
                {
                    this.ignores[type].Add(prop);
                }
            }

            /// <summary>
            /// Renames a property for a given type during serialization.
            /// </summary>
            /// <param name="type">The type containing the property.</param>
            /// <param name="propertyName">The original property name.</param>
            /// <param name="newJsonPropertyName">The new JSON property name.</param>
            public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
            {
                if (!this.renames.ContainsKey(type))
                {
                    this.renames[type] = new Dictionary<string, string>();
                }

                this.renames[type][propertyName] = newJsonPropertyName;
            }

            /// <summary>
            /// Creates a <see cref="JsonProperty"/> for the specified member, applying ignore and rename rules as configured.
            /// </summary>
            /// <param name="member">The member information.</param>
            /// <param name="memberSerialization">The member serialization mode.</param>
            /// <returns>The created <see cref="JsonProperty"/>.</returns>
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (property.DeclaringType is not null && property.PropertyName is not null)
                {
                    if (this.IsIgnored(property.DeclaringType, property.PropertyName))
                    {
                        property.ShouldSerialize = _ => false;
                        property.Ignored = true;
                    }

                    if (this.IsRenamed(property.DeclaringType, property.PropertyName, out var newName))
                    {
                        property.PropertyName = newName;
                    }
                }

                return property;
            }

            /// <summary>
            /// Determines whether the specified property should be ignored for the given type.
            /// </summary>
            /// <param name="type">The type to check.</param>
            /// <param name="jsonPropertyName">The property name to check.</param>
            /// <returns><c>true</c> if the property should be ignored; otherwise, <c>false</c>.</returns>
            private bool IsIgnored(Type type, string jsonPropertyName)
            {
                return this.ignores.TryGetValue(type, out var set) && set.Contains(jsonPropertyName);
            }

            /// <summary>
            /// Determines whether the specified property should be renamed for the given type.
            /// </summary>
            /// <param name="type">The type to check.</param>
            /// <param name="jsonPropertyName">The property name to check.</param>
            /// <param name="newJsonPropertyName">When this method returns, contains the new property name if renamed; otherwise, <c>null</c>.</param>
            /// <returns><c>true</c> if the property should be renamed; otherwise, <c>false</c>.</returns>
            private bool IsRenamed(Type type, string jsonPropertyName, out string? newJsonPropertyName)
            {
                if (this.renames.TryGetValue(type, out var renames) && renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
                {
                    return true;
                }

                newJsonPropertyName = null;
                return false;
            }
        }
    }
}
