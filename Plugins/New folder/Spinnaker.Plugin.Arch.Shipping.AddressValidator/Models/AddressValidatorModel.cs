using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator.Models;
public class AddressValidatorModel
{
    public string GoogleApiKey { get; set; }
    public string CountryCode { get; set; }
}
