using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Helpers;
public class TimeSpanParts
{
    public ITimeProvider TimeProvider { get; set; }

    public int Hours { get; set; }

    public int Minutes { get; set; }

    public int Seconds { get; set; }

    public static TimeSpanParts SecondsFromNow(ITimeProvider timeProvider, int n)
    {
        DateTime now = timeProvider.Now;
        return TimeSpanParts.New(now.Hour, now.Minute, now.Second + n);
    }

    public static TimeSpanParts MinutesFromNow(ITimeProvider timeProvider, int n)
    {
        DateTime now = timeProvider.Now;
        return TimeSpanParts.New(now.Hour, now.Minute + n, now.Second);
    }

    public static TimeSpanParts HoursFromNow(ITimeProvider timeProvider, int n)
    {
        DateTime now = timeProvider.Now;
        return TimeSpanParts.New(now.Hour + n, now.Minute, now.Second);
    }

    public static TimeSpanParts New(int hours, int minutes, int seconds) => TimeSpanParts.New(string.Format("{0}:{1}:{2}", (object)hours, (object)minutes, (object)seconds));

    public static TimeSpanParts New(string time) => new TimeSpanParts(time);

    public DateTime FromNow
    {
        get
        {
            DateTime now = this.TimeProvider.Now;
            DateTime dateTime = new DateTime(now.Year, now.Month, now.Day);
            dateTime = dateTime.AddHours((double)this.Hours);
            dateTime = dateTime.AddMinutes((double)this.Minutes);
            return dateTime.AddSeconds((double)this.Seconds);
        }
    }

    public TimeSpanParts(string time)
    {
        string[] strArray = time.Split(':');
        this.Hours = Math.Abs(int.Parse(strArray[0]));
        this.Minutes = Math.Abs(int.Parse(strArray[1]));
        this.Seconds = Math.Abs(int.Parse(strArray[2]));
        this.TimeProvider = (ITimeProvider)new TimeProvider();
    }

    public int TotalSeconds => this.Hours * 3600 + this.Minutes * 60 + this.Seconds;

    public int TotalMilliseconds => this.TotalSeconds * 1000;
}
