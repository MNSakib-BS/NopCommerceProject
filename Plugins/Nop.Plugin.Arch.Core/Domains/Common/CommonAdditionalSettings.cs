using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Common;
public class CommonAdditionalSettings: ISettings
{
    /// <summary>
    /// Gets or sets whether a user can attach a file to the contact us page.
    /// </summary>
    public bool DisplayContactUsAttachment { get; set; }

    /// <summary>
    /// Gets or sets the max file size of attachment on contact us page.
    /// </summary>
    public int ContactUsAttachmentFileSize { get; set; }

    /// <summary>
    /// Gets or sets whether the user gets shown the advanced contact us page.
    /// </summary>
    public bool DisplayAdvancedContactUsPage { get; set; }
}
