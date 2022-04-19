using BlazorSMHI.Shared.Entities;

namespace BlazorSMHI.Client.Services
{
    public interface IMCDataService
    {
        Task<IEnumerable<Machine>> GetAllMachines();
        Task<Machine?> GetMachineDetails(Guid machineId);
        Task<Machine?> AddMachine(Machine machine);
        Task UpdateMachine(Machine machine);
        Task DeleteMachine(Guid machineId);
        Task<IEnumerable<MachineType>> GetAllMachineTypes();
        Task<IEnumerable<MachineLocation>> GetAllMachineLocations();
        Task<IEnumerable<MachineData>> GetNewMachineData(Guid machineId);
    }
}
