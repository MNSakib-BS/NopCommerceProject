using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders;
using Nop.Plugin.Arch.Core.Domains.Affiliates;

namespace Nop.Plugin.Arch.Core.Data.Mapping.AbandonedCartReminders;
public class AbandonedCartReminderQueueBuilder : NopEntityBuilder<AbandonedCartReminderQueue>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
             .WithColumn(nameof(AbandonedCartReminderQueue.CustomerId)).AsInt32().ForeignKey<Customer>()
             .WithColumn(nameof(AbandonedCartReminderQueue.ReminderId)).AsInt32().ForeignKey<AbandonedCartReminder>()
             .WithColumn(nameof(AbandonedCartReminderQueue.StoreId)).AsInt32().ForeignKey<Store>();
    }
}
