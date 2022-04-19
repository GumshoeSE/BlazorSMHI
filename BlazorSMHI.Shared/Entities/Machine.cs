using System.ComponentModel.DataAnnotations;

namespace BlazorSMHI.Shared.Entities
{
    public class Machine
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public MachineStatus Status { get; set; }

        public MachineLocation MachineLocation { get; set; } = new MachineLocation();
        public Guid MachineLocationId { get; set; }

        public MachineType MachineType { get; set; } = new MachineType();
        public Guid MachineTypeId { get; set; }

        public ICollection<MachineData> MachineData { get; set; } = new List<MachineData>();
    }
}
