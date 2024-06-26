using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Core;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;
using Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Areas.Admin.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Infrastructure;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder
{
    public class AbandonedCartReminderPlugin : BasePlugin, IPlugin, IAdminMenuPlugin
    {
        private readonly List<string> _widgetZones;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IPluginDefaults _pluginDefaults;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IMessageTemplateService _messageTemplateService;
        public AbandonedCartReminderPlugin(IWebHelper webHelper,
                            ILocalizationService localizationService,
                            IPluginDefaults pluginDefaults,
                            IRepository<EmailAccount> emailAccountRepository,
                            IMessageTemplateService messageTemplateService)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _pluginDefaults = pluginDefaults;
            _emailAccountRepository = emailAccountRepository;
            _messageTemplateService = messageTemplateService;
        }
        public bool HideInWidgetList => true;
        public string GetWidgetViewComponent(string widgetZone)
        {
            return "ProductViewTracker";
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsTop });

        }
        public override async Task InstallAsync()
        {
            await base.InstallAsync();
            await  _localizationService.AddOrUpdateLocaleResourceAsync("HeaderMenu.AbandonedCartReminder", "AbandonedCartReminder", (string)null);
            await _pluginDefaults.UpdatePluginResourcesAsync("", "");
            await InstallMessageTemplateAsync();
        } 

        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            SiteMapNode siteMapNode1 = rootNode.ChildNodes.FirstOrDefault<SiteMapNode>((Func<SiteMapNode, bool>)(x => x.SystemName == "Configuration"));
            if (siteMapNode1 != null)
            {
                var menuItem = new SiteMapNode()
                {
                    Title = "Abandoned Cart Reminder",
                    Visible = true,
                    IconClass = "fa fa-dot-circle-o",
                    Url = "/Admin/AbandonedCartReminder",
                    ControllerName = "AbandonedCartReminder",
                    ActionName = "Index",
                    SystemName = "AbandonedCartReminder",
                    RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
                };

                siteMapNode1.ChildNodes.Add(menuItem);
            }
            return Task.CompletedTask;
        }
        public async Task InstallMessageTemplateAsync()
        {
            var messageTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.AbandonedCartReminderNotification);
            if (messageTemplates == null || (messageTemplates != null && !messageTemplates.Any()))
            {
                var emailAccountId = 0;
                var eaGeneral = await _emailAccountRepository.Table.FirstOrDefaultAsync();
                if (eaGeneral != null)
                {
                    emailAccountId = eaGeneral.Id;
                }

                var messageTemplate = new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.AbandonedCartReminderNotification,
                    Subject = "You have items in your cart",
                    Body = "<p>You have the following items in your cart<br />" +
                            "%ShoppingCart.Product(s)% {Environment.NewLine}<br /><br />" +
                            "<a href=\"%ShoppingCart.CartUrl%\">%Store.Name%</a><br />{Environment.NewLine}<br /></p>",
                    IsActive = true,
                    EmailAccountId = emailAccountId
                };

                await _messageTemplateService.InsertMessageTemplateAsync(messageTemplate);
            }
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        /// <summary>
        /// Update the plugin
        /// </summary>
        public override async Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await base.UpdateAsync(currentVersion, targetVersion);
            await _pluginDefaults.UpdatePluginResourcesAsync(currentVersion, targetVersion);
        }

    }
}
