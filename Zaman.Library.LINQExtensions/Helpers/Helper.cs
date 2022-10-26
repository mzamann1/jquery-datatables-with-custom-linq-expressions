using System.Reflection;

namespace Zaman.Library.LINQExtensions.Helpers
{
    public static class Helper
    {
        public static bool IsNumericType(Type type)
        {
            HashSet<Type> numericTypes = new() { typeof(int), typeof(double), typeof(decimal), typeof(long), typeof(short), typeof(sbyte), typeof(byte), typeof(ulong), typeof(ushort), typeof(uint), typeof(float) };
            return numericTypes.Contains(type) || numericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        public static bool IsNumericValue(object value)
        {
            var x = value;
            return value is byte or short or int or long or sbyte or ushort or uint or ulong or decimal or double or float;
        }

        public static object GetMaxValueIfOutOfRange(object value, Type type)
        {
            if (!IsNumericValue(value) || !IsNumericType(type))
            {
                // no need to check in range if value or type is not numeric
                return value;
            }

            try
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GenericTypeArguments[0];
                }

                dynamic currentValue = value;
                dynamic maxValue = type.GetField("MaxValue", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                if (currentValue > maxValue)
                {
                    return (ValueType)maxValue;
                }

                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public static Type GetPropertyType(Type type, string propName)
        {
            var property = type.GetProperty(propName);

            if (property == null)
            {
                return default;
            }

            return property.PropertyType;
        }
    }
}
