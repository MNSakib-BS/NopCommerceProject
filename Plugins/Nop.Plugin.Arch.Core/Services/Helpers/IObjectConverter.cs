namespace Nop.Plugin.Arch.Core.Services.Helpers;

public interface IObjectConverter
{
    T ToObject<T>(ICollection<KeyValuePair<string, string>> source)
        where T : class, new();

    T ToType<T>(string value);
}
