using System.Collections.Generic;
using Nop.Plugin.Api.DTO;

namespace Nop.Plugin.Api.Services
{
    public interface IProductAttributeConverter
    {
        Task<List<ProductItemAttributeDto>> ParseAsync(string attributesXml);
        Task<string> ConvertToXmlAsync(List<ProductItemAttributeDto> attributeDtos, int productId);
    }
}
