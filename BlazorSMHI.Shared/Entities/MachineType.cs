using System.ComponentModel.DataAnnotations;

namespace BlazorSMHI.Shared.Entities
{
    public class MachineType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            var item = obj as MachineType;
            if (item == null) return false;

            return item.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}