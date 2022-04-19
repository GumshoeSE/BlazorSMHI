using BlazorSMHI.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorSMHI.FuncApi.Helpers
{
    public interface ISMHIService
    {
        Task<List<MachineData>> GetPointForeCast(float longitude, float latitude);
    }
}
