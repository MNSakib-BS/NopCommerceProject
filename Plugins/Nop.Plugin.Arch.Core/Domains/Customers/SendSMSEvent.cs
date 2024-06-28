using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Customers;
public class SendSMSEvent
{
    public string CellNumber { get; private set; }
    public string Message { get; private set; }

    public SendSMSEvent(string cellnumber, string message)
    {
        CellNumber = cellnumber;
        Message = message;
    }
}