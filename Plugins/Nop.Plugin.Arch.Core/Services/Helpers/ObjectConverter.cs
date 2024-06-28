
using System.Reflection;
using Nop.Plugin.Arch.Core.Services.Helpers;

public class ObjectConverter : IObjectConverter
{
    private readonly ITypeConverter _TypeConverter;

    public ObjectConverter(ITypeConverter TypeConverter)
    {
        _TypeConverter = TypeConverter;
    }

    public T ToObject<T>(ICollection<KeyValuePair<string, string>> source)
        where T : class, new()
    {
        var someObject = new T();
        var someObjectType = someObject.GetType();

        if (source != null)
        {
            foreach (var item in source)
            {
                var itemKey = item.Key.Replace("_", string.Empty);

                var currentProperty = someObjectType.GetProperty(itemKey, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (currentProperty != null)
                {
                    currentProperty.SetValue(someObject, To(item.Value, currentProperty.PropertyType), null);
                }
            }
        }

        return someObject;
    }

    public T ToType<T>(string value)
    {
        return (T)To(value, typeof(T));
    }

    private object To(string value, Type type)
    {
        if (type == typeof(DateTime?))
        {
            return _TypeConverter.ToUtcDateTimeNullable(value);
        }
        if (type == typeof(int?))
        {
            return _TypeConverter.ToIntNullable(value);
        }
        if (type == typeof(int))
        {
            return _TypeConverter.ToInt(value);
        }
        if (type == typeof(List<int>))
        {
            return _TypeConverter.ToListOfInts(value);
        }
        if (type == typeof(bool?))
        {
            // Because currently status is the only boolean and we need to accept published and unpublished statuses.
            return _TypeConverter.ToStatus(value);
        }
        if (IsNullableEnum(type))
        {
            return _TypeConverter.ToEnumNullable(value, type);
        }

        // It should be the last resort, because it is not exception safe.
        return Convert.ChangeType(value, type);
    }

    private bool IsNullableEnum(Type t)
    {
        var u = Nullable.GetUnderlyingType(t);
        return u != null && u.IsEnum;
    }
}
