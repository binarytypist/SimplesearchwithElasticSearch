using Abp.Application.Services;
using Abp.Application.Services.Dto;
using GeorgDuncker.Vessels;
using Nest;
using System;
using System.Threading.Tasks;

namespace GeorgDuncker.Vessels
{
    public interface IVesselSearchAppService : IApplicationService
    {
        Task<ISearchResponse<VesselMappingDocument>> SearchVesselAsync(string query);
    }
}
