using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Helpers;
public interface ITimeProvider
{
    DateTime Now { get; }

    DateTime UtcNow { get; }
}

public class TimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}

public class MockTimeProvider : ITimeProvider
{
    public MockTimeProvider()
    {
        this.Now = DateTime.Now;
        this.UtcNow = DateTime.UtcNow;
    }

    public void Return(DateTime date)
    {
        this.Now = date;
        this.UtcNow = date.ToUniversalTime();
    }

    public DateTime Now { get; private set; }

    public DateTime UtcNow { get; private set; }
}
