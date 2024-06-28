using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Plugin.NopStation.Core
{
    public class NopStationCoreSettings : ISettings
    {
        public NopStationCoreSettings()
        {
            ActiveNopStationSystemNames = new List<string>();
            LicenseStrings = new List<string>();
        }
        
        public List<string> ActiveNopStationSystemNames { get; set; }

        public List<string> LicenseStrings { get; set; }
    }
}
