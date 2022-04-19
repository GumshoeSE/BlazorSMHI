using System.ComponentModel.DataAnnotations;

namespace BlazorSMHI.Shared.Entities
{
    public class MachineData
    {
        public Guid Id { get; set; }

        [Required]
        public double Data { get; set; }

        [Required]
        public DateTime Time { get; set; }

        public Guid MachineId { get; set; }
    }
}