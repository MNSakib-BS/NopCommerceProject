namespace Nop.Plugin.Arch.Core.Services.Helpers;
public interface ITypeConverter
{
    DateTime? ToUtcDateTimeNullable(string value);
    int ToInt(string value);
    int? ToIntNullable(string value);
    IList<int> ToListOfInts(string value);
    bool? ToStatus(string value);
    object ToEnumNullable(string value, Type type);
}
