using BlazorSMHI.Shared.Entities;
using Microsoft.Azure.Cosmos.Table;

namespace BlazorSMHI.FuncApi.Models
{
    public class MachineTableEntity : TableEntity
    {
        public string Name { get; set; }
        public int Status { get; set; } = (int)MachineStatus.Offline;
        public string LocationId { get; set; }
        public string LocationCountry { get; set; }
        public string LocationCity { get; set; }
        public string LocationLongitude { get; set; }
        public string LocationLatitude { get; set; }
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
    }
}
