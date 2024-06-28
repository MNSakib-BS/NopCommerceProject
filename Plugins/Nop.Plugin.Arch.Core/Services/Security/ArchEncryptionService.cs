using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Services.Security;
public class ArchEncryptionService : IArchEncryptionService
{
    #region Fields

    #endregion

    #region Ctor

    #endregion

    #region Methods

    public virtual string ComputeSha256Hash(string rawData)
    {
        using (var sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            var builder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    #endregion

}
