using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Topics;

namespace Nop.Plugin.Arch.Core.Services.Topics;
public interface ITopicAdditionalService
{
    Task<TopicAdditional> GetTopicAdditionalByTopicIdAsync(int topicId);
    Task<TopicAdditional> GetTopicAdditionalByIdAsync(int id);
    Task DeleteTopicAdditionalAsync(TopicAdditional topicAdditional);
    Task InsertTopicAdditionalAsync(TopicAdditional topicAdditional);
    Task UpdateTopicAdditionalAsync(TopicAdditional topicAdditional);

}
