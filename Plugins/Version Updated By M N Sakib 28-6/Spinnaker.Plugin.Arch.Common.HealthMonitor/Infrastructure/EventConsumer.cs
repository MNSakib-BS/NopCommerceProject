using Nop.Web.Framework.Models;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Logging;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Spinnaker.Plugin.Arch.Common.HealthMonitor.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using Nop.Plugin.Arch.Core.Domains.Logging;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure
{
    public class HealthEventConsumer : IConsumer<ErrorHandledEvent>
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        public HealthEventConsumer(IStoreContext storeContext, ISettingService settingService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }
        public async Task HandleEventAsync(ErrorHandledEvent eventMessage)
        {
            var setting = await _settingService.LoadSettingAsync<HealthMonitorSettings>();
            if (string.IsNullOrEmpty(setting.SiteKey) || string.IsNullOrEmpty(setting.MonitoringHostURL))
                return;

            var exceptionDetail = ExtractExceptionText(eventMessage.Exception);
            var item = new LogEvent
            {
                Level = "Error",
                RenderedMessage = eventMessage.ErrorMessage,
                Timestamp = DateTime.UtcNow,
                Exception = exceptionDetail,
                SiteKey = setting.SiteKey
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(setting.MonitoringHostURL.TrimEnd('/'));
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                string data = JsonConvert.SerializeObject(item);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/logevents/logexception", content);
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<object>(await response.Content.ReadAsStringAsync());
            }
        }



        private static string ExtractExceptionText(Exception exception)
        {
            StringBuilder StringBuilder = new StringBuilder();

            try
            {
                StringBuilder.Append("[Exception Details]" + Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);
                StringBuilder.Append(ExceptionDetailToString(exception) + Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);

                StringBuilder.Append(ExtractInnerException(exception));

                StringBuilder.Append("[Stack Trace]");
                StringBuilder.Append(Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);
                StringBuilder.Append(exception.StackTrace?.ToString());
                StringBuilder.Append(Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);

                if (exception is ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    StringBuilder.Append(ExtractLoaderExceptions(reflectionTypeLoadException));
                    StringBuilder.Append(Environment.NewLine);
                    StringBuilder.Append(Environment.NewLine);
                }
            }
            catch (Exception)
            { }

            return StringBuilder.ToString();
        }

        private static string ExtractInnerException(Exception exception)
        {
            StringBuilder StringBuilder = new StringBuilder();

            if (exception.InnerException != null)
            {
                StringBuilder.Append("[Inner Exception Start]" + Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);
                StringBuilder.Append(ExceptionDetailToString(exception.InnerException) + Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);
                StringBuilder.Append(ExtractInnerException(exception.InnerException));
                StringBuilder.Append("[Inner Exception End]" + Environment.NewLine);
                StringBuilder.Append(Environment.NewLine);
            }

            return StringBuilder.ToString();
        }

        private static string ExtractLoaderExceptions(ReflectionTypeLoadException exception)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (exception.LoaderExceptions != null && exception.LoaderExceptions.Any())
            {
                stringBuilder.Append("[Loader Exception Start]" + Environment.NewLine);
                stringBuilder.Append(Environment.NewLine);
                foreach (var loaderException in exception.LoaderExceptions)
                    stringBuilder.Append(ExceptionDetailToString(loaderException) + Environment.NewLine);
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("[Loader Exception End]" + Environment.NewLine);
                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        private static string ExceptionDetailToString(Exception excpetion)
        {
            StringBuilder StringBuilder = new StringBuilder();

            try
            {

                StringBuilder.Append("[" + excpetion.GetType().FullName + "]");
                StringBuilder.Append(Environment.NewLine);

                StringBuilder.Append("ExceptionSource: ");
                StringBuilder.Append(excpetion.Source);
                StringBuilder.Append(Environment.NewLine);

                StringBuilder.Append("ExceptionMessage: ");
                StringBuilder.Append(excpetion.Message);
                StringBuilder.Append(Environment.NewLine);

                StringBuilder.Append("ExceptionTargetSite: ");
                StringBuilder.Append(excpetion.TargetSite?.Name);

            }
            catch (Exception)
            { }

            return StringBuilder.ToString();
        }

        
    }
}
