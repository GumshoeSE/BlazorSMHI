using BlazorSMHI.FuncApi.Models;
using BlazorSMHI.Shared.Entities;
using System;
using System.Collections.Generic;

namespace BlazorSMHI.FuncApi.Helpers
{
    public static class Mapper
    {
        public static MachineTableEntity ToTableEntity(this Machine machine)
        {
            return new MachineTableEntity
            {
                Name = machine.Name,
                Status = (int)machine.Status,
                LocationId = machine.MachineLocationId.ToString(),
                LocationCity = machine.MachineLocation.City,
                LocationCountry = machine.MachineLocation.Country,
                LocationLongitude = machine.MachineLocation.Longitude.ToString(),
                LocationLatitude = machine.MachineLocation.Latitude.ToString(),
                TypeId = machine.MachineTypeId.ToString(),
                TypeDescription = machine.MachineType.Description,
                TypeName = machine.MachineType.Name,
                PartitionKey = "Machine",
                RowKey = machine.Id.ToString()
            };
        }

        public static Machine ToMachine(this MachineTableEntity machineTable)
        {
            return new Machine
            {
                Id = Guid.Parse(machineTable.RowKey),
                Name = machineTable.Name,
                Status = (MachineStatus)machineTable.Status,
                MachineLocationId = Guid.Parse(machineTable.LocationId),
                MachineLocation = new MachineLocation
                {
                    Id = Guid.Parse(machineTable.LocationId),
                    City = machineTable.LocationCity,
                    Country = machineTable.LocationCountry,
                    Longitude = float.Parse(machineTable.LocationLongitude),
                    Latitude = float.Parse(machineTable.LocationLatitude)
                },
                MachineTypeId = Guid.Parse(machineTable.TypeId),
                MachineType = new MachineType
                {
                    Id = Guid.Parse(machineTable.TypeId),
                    Name = machineTable.TypeName,
                    Description = machineTable.TypeDescription
                },
                MachineData = new List<MachineData>()
            };
        }
    }
}
