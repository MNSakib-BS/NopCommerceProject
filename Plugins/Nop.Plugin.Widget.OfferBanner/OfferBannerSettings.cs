using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widget.OfferBanner
{
    public class OfferBannerSettings : ISettings 
    {
        public string Message { get; set; }
    }
}
