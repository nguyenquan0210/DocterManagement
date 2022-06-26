using DoctorManagement.ViewModels.Catalog.Topic;
using DoctorManagement.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface ITopicApiClient
    {
        Task<ApiResult<bool>> Create(TopicCreateRequest request);

        Task<ApiResult<bool>> Update(TopicUpdateRequest request);

        Task<int> Delete(Guid Id);

        Task<ApiResult<PagedResult<TopicVm>>> GetAllPaging(GetTopicPagingRequest request);

        Task<ApiResult<List<TopicVm>>> GetAll();

        Task<ApiResult<TopicVm>> GetById(Guid Id);
    }
}
