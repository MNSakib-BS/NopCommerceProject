using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Topics;
public class TopicAdditional:BaseEntity
{
    public int TopicId { get; set; }
    /// <summary>
    /// Gets or sets the value indicating whether this topic should be included in terms and conditions
    /// </summary>
    public bool IncludeInTermsAndConditions { get; set; }
    public string? ExternalUrl { get; set; }

}
