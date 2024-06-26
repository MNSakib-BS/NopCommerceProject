using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Spinnaker.Plugin.Arch.Shipping.AddressPicker.Components;

public class AddressPickerViewComponent : NopViewComponent
{
    public AddressPickerViewComponent(){}
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        var model = string.Empty;
        return View("~/Plugins/Arch.Addresspicker/Views/AddressPicker.cshtml", model);
    }
}
