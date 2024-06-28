using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Topics;

namespace Nop.Plugin.Arch.Core.Services.Topics;
public class TopicAdditionalService : ITopicAdditionalService
{
    #region Fields

    private readonly IRepository<TopicAdditional> _topicAdditionalRepository;

    #endregion

    #region Ctor

    public TopicAdditionalService(IRepository<TopicAdditional> topicAdditionalRepository)
    {
        _topicAdditionalRepository = topicAdditionalRepository;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual async Task DeleteTopicAdditionalAsync(TopicAdditional topicAdditional)
    {
        await _topicAdditionalRepository.DeleteAsync(topicAdditional);
    }

    public virtual async Task<TopicAdditional> GetTopicAdditionalByIdAsync(int id)
    {
        return await _topicAdditionalRepository.GetByIdAsync(id, cache => default, false);
    }

    public virtual async Task<TopicAdditional> GetTopicAdditionalByTopicIdAsync(int topicId)
    {
        return await _topicAdditionalRepository.Table.Where(e => e.TopicId == topicId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertTopicAdditionalAsync(TopicAdditional topicAdditional)
    {
        await _topicAdditionalRepository.InsertAsync(topicAdditional);
    }

    public virtual async Task UpdateTopicAdditionalAsync(TopicAdditional topicAdditional)
    {
        await _topicAdditionalRepository.UpdateAsync(topicAdditional);
    }

    #endregion
}
