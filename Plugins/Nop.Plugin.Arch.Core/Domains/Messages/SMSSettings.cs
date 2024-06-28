using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Messages;
public  class SMSSettings : ISettings
{
    public string? SMSMethod { get; set; }
    public int OtpMinLength { get; set; }
    public int OtpMaxLength { get; set; }
    public string? BearerToken { get; set; }
}
