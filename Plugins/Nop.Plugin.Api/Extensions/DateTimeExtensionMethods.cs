using System;

namespace Nop.Plugin.Api.Extensions
{
    public static class DateTimeExtensionMethods
    {
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddTicks(-1);
        }
    }
}