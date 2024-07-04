using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Helpers
{
    public interface IMappingHelper
    {
        Task MergeAsync(object source, object destination);
        Task SetValuesAsync(
            Dictionary<string, object> propertyNameValuePairs, object objectToBeUpdated,
            Type objectToBeUpdatedType, Dictionary<object, object> objectPropertyNameValuePairs,
            bool handleComplexTypeCollections = false);
    }
}
