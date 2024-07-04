using System.Collections.Generic;
using System.IO;

namespace Nop.Plugin.Api.Helpers
{
    public interface IJsonHelper
    {
        Task<Dictionary<string, object>> GetRequestJsonDictionaryFromStreamAsync(Stream stream, bool rewindStream);
        Task<string> GetRootPropertyNameAsync<T>() where T : class, new();
    }
}
