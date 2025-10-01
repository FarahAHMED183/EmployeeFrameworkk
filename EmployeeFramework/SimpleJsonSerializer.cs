using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EmployeeFramework
{
    public static class SimpleJsonSerializer
    {
        public static string Serialize(object obj)
        {
            if (obj == null)
                return "null";

            var type = obj.GetType();

            if (type == typeof(string))
                return $"\"{EscapeString(obj.ToString())}\"";

            if (type == typeof(bool))
                return obj.ToString().ToLower();

            if (IsNumericType(type))
                return obj.ToString();

            if (type == typeof(DateTime))
                return $"\"{((DateTime)obj).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\"";

            if (type == typeof(DateTime?))
            {
                var dateTime = (DateTime?)obj;
                return dateTime.HasValue ? $"\"{dateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}\"" : "null";
            }

            if (type == typeof(Guid))
                return $"\"{obj}\"";

            if (type == typeof(decimal))
                return ((decimal)obj).ToString(CultureInfo.InvariantCulture);

            if (obj is IEnumerable enumerable && type != typeof(string))
            {
                var items = new List<string>();
                foreach (var item in enumerable)
                {
                    items.Add(Serialize(item));
                }
                return $"[{string.Join(",", items)}]";
            }

            var properties = new List<string>();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var prop in props)
            {
                if (prop.CanRead)
                {
                    var value = prop.GetValue(obj);
                    var serializedValue = Serialize(value);
                    properties.Add($"\"{ToCamelCase(prop.Name)}\":{serializedValue}");
                }
            }

            return $"{{{string.Join(",", properties)}}}";
        }

        public static T Deserialize<T>(string json) where T : new()
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);

            json = json.Trim();
            if (!json.StartsWith("{") || !json.EndsWith("}"))
                throw new ArgumentException("Invalid JSON format");

            var result = new T();
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var content = json.Substring(1, json.Length - 2);
            var pairs = SplitJsonPairs(content);

            foreach (var pair in pairs)
            {
                var colonIndex = pair.IndexOf(':');
                if (colonIndex > 0)
                {
                    var key = pair.Substring(0, colonIndex).Trim().Trim('"');
                    var value = pair.Substring(colonIndex + 1).Trim();

                    var property = properties.FirstOrDefault(p => 
                        string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(ToCamelCase(p.Name), key, StringComparison.OrdinalIgnoreCase));

                    if (property != null && property.CanWrite)
                    {
                        var convertedValue = ConvertValue(value, property.PropertyType);
                        property.SetValue(result, convertedValue);
                    }
                }
            }

            return result;
        }

        private static List<string> SplitJsonPairs(string content)
        {
            var pairs = new List<string>();
            var current = new StringBuilder();
            var inString = false;
            var braceLevel = 0;

            for (int i = 0; i < content.Length; i++)
            {
                var c = content[i];

                if (c == '"' && (i == 0 || content[i - 1] != '\\'))
                {
                    inString = !inString;
                }

                if (!inString)
                {
                    if (c == '{' || c == '[')
                        braceLevel++;
                    else if (c == '}' || c == ']')
                        braceLevel--;
                    else if (c == ',' && braceLevel == 0)
                    {
                        pairs.Add(current.ToString().Trim());
                        current.Clear();
                        continue;
                    }
                }

                current.Append(c);
            }

            if (current.Length > 0)
                pairs.Add(current.ToString().Trim());

            return pairs;
        }

        private static object ConvertValue(string value, Type targetType)
        {
            if (value == "null")
                return null;

            value = value.Trim();

            if (targetType == typeof(string))
                return value.Trim('"').Replace("\\\"", "\"");

            if (targetType == typeof(int) || targetType == typeof(int?))
                return int.Parse(value);

            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                return decimal.Parse(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return bool.Parse(value);

            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return DateTime.Parse(value.Trim('"'));

            if (targetType == typeof(Guid) || targetType == typeof(Guid?))
                return Guid.Parse(value.Trim('"'));

            return value;
        }

        private static bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) ||
                   type == typeof(short) || type == typeof(byte) || type == typeof(uint) || type == typeof(ulong) ||
                   type == typeof(ushort) || type == typeof(sbyte);
        }

        private static string EscapeString(string input)
        {
            return input.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }

        private static string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return char.ToLowerInvariant(input[0]) + input.Substring(1);
        }
    }
}