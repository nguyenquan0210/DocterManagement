using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.System.StatisticService
{
    public interface IStatisticService
    {
        Task<ApiResult<List<HistoryActiveVm>>> ListActiveUser();
        Task<ApiResult<bool>> AddActiveUser(HistoryActiveCreateRequest request);
    }
}
