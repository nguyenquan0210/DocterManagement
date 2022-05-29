using DoctorManagement.ViewModels.Common;
using DoctorManagement.ViewModels.System.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ApiIntegration
{
    public interface IStatisticApiClient
    {
        Task<List<StatisticNews>> GetServiceFeeStatiticYear(GetHistoryActivePagingRequest request);
        Task<List<StatisticNews>> GetServiceFeeStatiticDay(GetHistoryActivePagingRequest request);
        Task<List<StatisticNews>> GetServiceFeeStatiticMonth(GetHistoryActivePagingRequest request);
    }
}
