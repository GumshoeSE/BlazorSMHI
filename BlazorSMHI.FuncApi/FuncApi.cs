using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using Microsoft.Azure.Storage.Blob;
using BlazorSMHI.Shared.Entities;
using BlazorSMHI.FuncApi.Helpers;
using BlazorSMHI.FuncApi.Models;

namespace BlazorSMHI.FuncApi
{
    public class FuncApi
    {
        private readonly ISMHIService _smhi;

        public FuncApi(ISMHIService smhi)
        {
            _smhi = smhi;
        }

        [FunctionName("Create")]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "machines")] HttpRequest req,
            [Table("machines", Connection = "AzureWebJobsStorage")] IAsyncCollector<MachineTableEntity> machineTable,
            ILogger log)
        {
            log.LogInformation("Add new Machine to storage");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var machine = JsonConvert.DeserializeObject<Machine>(requestBody);
            machine.Id = machine.Id == Guid.Empty ? Guid.NewGuid() : machine.Id;

            if (machine is null) return new BadRequestResult();

            await machineTable.AddAsync(machine.ToTableEntity());

            return new OkObjectResult(machine);
        }

        [FunctionName("Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machines")] HttpRequest req,
            [Table("machines", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Get all machines");

            var query = new TableQuery<MachineTableEntity>();
            var res = await table.ExecuteQuerySegmentedAsync(query, null);

            var response = res.Select(Mapper.ToMachine).ToList();
            foreach (var machine in response)
            {
                machine.MachineData = await _smhi.GetPointForeCast(machine.MachineLocation.Longitude, machine.MachineLocation.Latitude);
            }

            return new OkObjectResult(response);
        }

        [FunctionName("GetTypes")]
        public static async Task<IActionResult> GetTypes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machinetypes")] HttpRequest req,
            [Table("machines", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Get all machine types");

            var query = new TableQuery<MachineTableEntity>();
            var res = await table.ExecuteQuerySegmentedAsync(query, null);

            var response = res.Select(Mapper.ToMachine).Select(m => m.MachineType).Distinct().ToList();

            if (!response.Exists(t => t.Name == "Temperature Sensor"))
            {
                response.Add(new MachineType
                {
                    Id = Guid.NewGuid(),
                    Name = "Temperature Sensor",
                    Description = "Temperature in Celsius"
                });
            }

            return new OkObjectResult(response);
        }

        [FunctionName("GetLocations")]
        public static async Task<IActionResult> GetLocations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machinelocations")] HttpRequest req,
            [Table("machines", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Get all machine locations");

            var query = new TableQuery<MachineTableEntity>();
            var res = await table.ExecuteQuerySegmentedAsync(query, null);

            var response = res.Select(Mapper.ToMachine).Select(m => m.MachineLocation).Distinct().ToList();

            if (!response.Exists(l => l.City == "Stockholm"))
            {
                response.Add(new MachineLocation
                {
                    Id = Guid.NewGuid(),
                    Country = "Sweden",
                    City = "Stockholm",
                    Longitude = 18.0686f,
                    Latitude = 59.3293f
                });
            }
            if (!response.Exists(l => l.City == "Göteborg"))
            {
                response.Add(new MachineLocation
                {
                    Id = Guid.NewGuid(),
                    Country = "Sweden",
                    City = "Göteborg",
                    Longitude = 11.975f,
                    Latitude = 57.709f
                });
            }
            if (!response.Exists(l => l.City == "Luleå"))
            {
                response.Add(new MachineLocation
                {
                    Id = Guid.NewGuid(),
                    Country = "Sweden",
                    City = "Luleå",
                    Longitude = 22.157f,
                    Latitude = 65.585f
                });
            }

            return new OkObjectResult(response);
        }

        [FunctionName("GetDetails")]
        public async Task<IActionResult> GetDetails(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machines/{id}")] HttpRequest req,
            [Table("machines", "Machine", "{id}", Connection = "AzureWebJobsStorage")] MachineTableEntity machineTableToReturn,
            string id,
            ILogger log)
        {
            log.LogInformation("Get details for machine");

            if (machineTableToReturn is null || machineTableToReturn.RowKey != id) return new NotFoundResult();

            var response = machineTableToReturn.ToMachine();
            var forecast = await _smhi.GetPointForeCast(response.MachineLocation.Longitude, response.MachineLocation.Latitude);
            response.MachineData = forecast;

            return new OkObjectResult(response);
        }

        [FunctionName("GetNewData")]
        public async Task<IActionResult> GetNewData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "machinedata/{id}/new")] HttpRequest req,
            [Table("machines", "Machine", "{id}", Connection = "AzureWebJobsStorage")] MachineTableEntity machineTableEntity,
            string id,
            ILogger log)
        {
            log.LogInformation("Get new data for machine");

            if (machineTableEntity is null || machineTableEntity.RowKey != id) return new NotFoundResult();

            var machine = machineTableEntity.ToMachine();
            var forecast = await _smhi.GetPointForeCast(machine.MachineLocation.Longitude, machine.MachineLocation.Latitude);

            return new OkObjectResult(forecast);
        }

        [FunctionName("Put")]
        public static async Task<IActionResult> Put(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "machines/{id}")] HttpRequest req,
            [Table("machines", Connection = "AzureWebJobsStorage")] CloudTable table,
            Guid id,
            ILogger log)
        {
            log.LogInformation("Update machine");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var machineToUpdate = JsonConvert.DeserializeObject<Machine>(requestBody);

            if (machineToUpdate is null || machineToUpdate.Id != id) return new BadRequestResult();

            var machineEntity = machineToUpdate.ToTableEntity();
            machineEntity.ETag = "*";

            var operation = TableOperation.Replace(machineEntity);
            await table.ExecuteAsync(operation);

            return new NoContentResult();
        }

        [FunctionName("Delete")]
        public static async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "machines/{id}")] HttpRequest req,
            [Table("machines", "Machine", "{id}", Connection = "AzureWebJobsStorage")] MachineTableEntity machineTableToDelete,
            [Table("machines", Connection = "AzureWebJobsStorage")] CloudTable table,
            [Queue("deletequeue", Connection = "AzureWebJobsStorage")] IAsyncCollector<MachineTableEntity> queue,
            ILogger log)
        {
            log.LogInformation("Delete machine");

            if (machineTableToDelete is null) return new BadRequestResult();

            var operation = TableOperation.Delete(machineTableToDelete);
            await table.ExecuteAsync(operation);

            // add to queue
            await queue.AddAsync(machineTableToDelete);

            return new NoContentResult();
        }

        [FunctionName("GetRemovedFromQueue")]
        public static async Task GetRemovedFromQueue(
          [QueueTrigger("deletequeue", Connection = "AzureWebJobsStorage")] MachineTableEntity item,
          [Blob("removed", Connection = "AzureWebJobsStorage")] CloudBlobContainer blobContainer,
          ILogger log)
        {
            log.LogInformation("Queue trigger started...");

            await blobContainer.CreateIfNotExistsAsync();
            var blob = blobContainer.GetBlockBlobReference($"{item.RowKey}.txt");
            await blob.UploadTextAsync($"{DateTime.Now}: {item.Name} was removed");
        }
    }
}
