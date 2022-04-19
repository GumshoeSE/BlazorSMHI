using BlazorSMHI.Shared.Entities;
using System.Text;
using System.Text.Json;

namespace BlazorSMHI.Client.Services
{
    public class MCDataService : IMCDataService
    {
        private readonly HttpClient _httpClient;

        public MCDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Machine?> AddMachine(Machine machine)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(machine), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/machines", jsonContent);

            Machine? responseContent = null;

            if (response.IsSuccessStatusCode)
            {
                responseContent = await JsonSerializer.DeserializeAsync<Machine>(await response.Content.ReadAsStreamAsync()) ??
                    throw new Exception("Received null from successful Post");
            }

            return responseContent;
        }

        public async Task DeleteMachine(Guid machineId)
        {
            await _httpClient.DeleteAsync($"api/machines/{machineId}");
        }

        public async Task<IEnumerable<MachineLocation>> GetAllMachineLocations()
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<MachineLocation>>
                (await _httpClient.GetStreamAsync($"api/machinelocations"),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<MachineLocation>();
        }

        public async Task<IEnumerable<Machine>> GetAllMachines()
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<Machine>>
                (await _httpClient.GetStreamAsync($"api/machines"),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<Machine>();
        }

        public async Task<IEnumerable<MachineType>> GetAllMachineTypes()
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<MachineType>>
                (await _httpClient.GetStreamAsync($"api/machinetypes"),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<MachineType>();
        }

        public async Task<Machine?> GetMachineDetails(Guid machineId)
        {
            return await JsonSerializer.DeserializeAsync<Machine>
                (await _httpClient.GetStreamAsync($"api/machines/{machineId}"),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<IEnumerable<MachineData>> GetNewMachineData(Guid machineId)
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<MachineData>>
                (await _httpClient.GetStreamAsync($"api/machinedata/{machineId}/new"),
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<MachineData>();
        }

        public async Task UpdateMachine(Machine machine)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(machine), Encoding.UTF8, "application/json");
            await _httpClient.PutAsync($"api/machines/{machine.Id}", jsonContent);
        }
    }
}
