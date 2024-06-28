using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Logging;
public class ErrorHandledEvent
{
    public string ErrorMessage { get; }
    public Exception Exception { get; }

    public ErrorHandledEvent(string errorMessage, Exception exception = null)
    {
        ErrorMessage = errorMessage;
        Exception = exception;
    }
}
