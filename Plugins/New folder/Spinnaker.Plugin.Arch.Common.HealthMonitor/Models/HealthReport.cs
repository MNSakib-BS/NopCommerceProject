using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthChecks.UI.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Models;
public class NopHealthReport
{
    public UIHealthStatus Status { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public Dictionary<string, UIHealthReportEntry> Entries { get; }

    public NopHealthReport(Dictionary<string, UIHealthReportEntry> entries, TimeSpan totalDuration)
    {
        Entries = entries ?? new Dictionary<string, UIHealthReportEntry>();
        TotalDuration = totalDuration;
    }

    public static NopHealthReport CreateFrom(HealthReport report, Func<Exception, string>? exceptionMessage = null)
    {
        var uiReport = new NopHealthReport(new Dictionary<string, UIHealthReportEntry>(), report.TotalDuration)
        {
            Status = (UIHealthStatus)report.Status,
        };

        foreach (var item in report.Entries)
        {
            var entry = new UIHealthReportEntry
            {
                Data = item.Value.Data,
                Description = item.Value.Description,
                Duration = item.Value.Duration,
                Status = (UIHealthStatus)item.Value.Status
            };

            if (item.Value.Exception != null)
            {
                var message = exceptionMessage == null ? item.Value.Exception?.Message : exceptionMessage(item.Value.Exception);

                entry.Exception = message;
                entry.Description = item.Value.Description ?? message;
            }

            entry.Tags = item.Value.Tags;

            uiReport.Entries.Add(item.Key, entry);
        }

        return uiReport;
    }

    public static NopHealthReport CreateFrom(Exception exception, string entryName = "Endpoint")
    {
        var uiReport = new NopHealthReport(new Dictionary<string, UIHealthReportEntry>(), TimeSpan.FromSeconds(0))
        {
            Status = UIHealthStatus.Unhealthy,
        };

        uiReport.Entries.Add(entryName, new UIHealthReportEntry
        {
            Exception = exception.Message,
            Description = exception.Message,
            Duration = TimeSpan.FromSeconds(0),
            Status = UIHealthStatus.Unhealthy
        });

        return uiReport;
    }
}
public enum UIHealthStatus
{
    Unhealthy = 0,
    Degraded = 1,
    Healthy = 2
}

public class UIHealthReportEntry
{
    public IReadOnlyDictionary<string, object> Data { get; set; } = null!;
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Exception { get; set; }
    public UIHealthStatus Status { get; set; }
    public IEnumerable<string>? Tags { get; set; }
}
