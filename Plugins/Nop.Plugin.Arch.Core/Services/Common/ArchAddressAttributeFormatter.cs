using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Attributes;
using Nop.Core.Domain.Common;
using Nop.Services.Html;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Nop.Plugin.Arch.Core.Services.Common;
/// <summary>
/// Address attribute helper
/// </summary>
public partial class ArchAddressAttributeFormatter : AttributeFormatter<AddressAttribute, AddressAttributeValue>
{

    #region Fields     

    private readonly bool _hasMultipleLanguages;

    #endregion

    #region Ctor

    public ArchAddressAttributeFormatter(IAttributeParser<AddressAttribute, AddressAttributeValue> attributeParser, IAttributeService<AddressAttribute, AddressAttributeValue> attributeService, IHtmlFormatter htmlFormatter, ILocalizationService localizationService, IWorkContext workContext) : base(attributeParser, attributeService, htmlFormatter, localizationService, workContext)
    {
        _hasMultipleLanguages = EngineContext.Current.Resolve<ILanguageService>()?.GetAllLanguages()?.Count > 1;
    }

    #endregion

    #region Methods

    #region Methods

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
    /// <returns>Attributes</returns>
    public virtual async Task<string> FormatAttributesAsync(string attributesXml,
        string separator = "<br />",
        bool htmlEncode = true)
    {
        var result = new StringBuilder();

        var currentLanguage=await _workContext.GetWorkingLanguageAsync();

        var attributes = await _attributeParser.ParseAttributesAsync(attributesXml);
        for (var i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];
            var valuesStr = _attributeParser.ParseValues(attributesXml, attribute.Id);
            for (var j = 0; j < valuesStr.Count; j++)
            {
                var valueStr = valuesStr[j];
                var formattedAttribute = string.Empty;
                if (!attribute.ShouldHaveValues)
                {
                    //no values
                    if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        //multiline textbox
                        var attributeName = _hasMultipleLanguages ? await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id) : attribute.Name;
                        //encode (if required)
                        if (htmlEncode)
                            attributeName = WebUtility.HtmlEncode(attributeName);
                        formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                        //we never encode multiline textbox input
                    }
                    else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                    {
                        //file upload
                        //not supported for address attributes
                    }
                    else
                    {
                        //other attributes (textbox, datepicker)
                        formattedAttribute = $"{(_hasMultipleLanguages ?await  _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id) : attribute.Name)}: {valueStr}";
                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                    }
                }
                else
                {
                    if (int.TryParse(valueStr, out var attributeValueId))
                    {
                        var attributeValue = await _attributeService.GetAttributeValueByIdAsync(attributeValueId);
                        if (attributeValue != null)
                        {
                            formattedAttribute = $"{(_hasMultipleLanguages ? await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id) : attribute.Name)}: {(_hasMultipleLanguages ? await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id) : attributeValue.Name)}";
                        }
                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                    }
                }

                if (string.IsNullOrEmpty(formattedAttribute))
                    continue;

                if (i != 0 || j != 0)
                    result.Append(separator);

                result.Append(formattedAttribute);
            }
        }

        return result.ToString();
    }

    #endregion

    #endregion


}
