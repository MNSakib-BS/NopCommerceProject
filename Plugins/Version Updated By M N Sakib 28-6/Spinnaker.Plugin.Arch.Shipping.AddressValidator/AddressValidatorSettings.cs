using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator;
public class AddressValidatorSettings : ISettings
{
    public string CountryRestriction { get; set; }
}
