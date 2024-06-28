using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Mobile;
public class MobileSettings : ISettings
{
    public int LogoPictureId { get; set; }
    public string? GoogleApiKey { get; set; }
    public string? PrimaryColor { get; set; }
    public string? PrimaryLightColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? SecondaryLightColor { get; set; }
    public string? ErrorColor { get; set; }
    public string? LineSeparatorColor { get; set; }
    public string? BackgroundColor { get; set; }
    public string? AdminChangesColor { get; set; }
    public string? JoinedSelectedShippingMethodIds { get; set; }
    public int OrderDownloadLimit { get; set; }
    public string? AppDownloadLink { get; set; }
}
