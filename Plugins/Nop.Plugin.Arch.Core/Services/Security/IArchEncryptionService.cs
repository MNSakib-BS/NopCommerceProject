using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Services.Security;
public interface IArchEncryptionService
{
    string ComputeSha256Hash(string rawData);
}
