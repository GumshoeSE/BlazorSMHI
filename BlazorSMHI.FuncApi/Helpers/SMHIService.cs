using BlazorSMHI.FuncApi.Models;
using BlazorSMHI.Shared.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorSMHI.FuncApi.Helpers
{
    public class SMHIService : ISMHIService
    {
        private readonly HttpClient _httpClient;

        public SMHIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<MachineData>> GetPointForeCast(float longitude, float latitude)
        {
            List<MachineData> data = new List<MachineData>();

            var lngStr = longitude.ToString().Replace(",", ".");
            var latStr = latitude.ToString().Replace(",", ".");

            var response = await JsonSerializer.DeserializeAsync<Forecast>
                (await _httpClient.GetStreamAsync($"/api/category/pmp3g/version/2/geotype/point/lon/{lngStr}/lat/{latStr}/data.json"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new Forecast();

            foreach (var series in response.TimeSeries)
            {
                foreach (var dataParam in series.Parameters)
                {
                    if (dataParam.Name == "t")
                    {
                        var temperature = dataParam.Values[0];
                        data.Add(new MachineData
                        {
                            Data = temperature,
                            Time = series.ValidTime.LocalDateTime
                        });
                        break;
                    }
                }
            }

            return data;
        }
    }
}
