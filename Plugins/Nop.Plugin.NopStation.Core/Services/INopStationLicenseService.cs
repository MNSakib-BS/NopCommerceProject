using Nop.Plugin.NopStation.Core.Infrastructure;

namespace Nop.Plugin.NopStation.Core.Services
{
    public interface INopStationLicenseService
    {
		bool IsLicensed();

        KeyVerificationResult VerifyProductKey(string key);
    }

    public class NopStationLicenseService : INopStationLicenseService
    {
        public bool IsLicensed()
        {
            return true;
        }

        public KeyVerificationResult VerifyProductKey(string key)
        {
            return KeyVerificationResult.Valid;
        }
    }
}
