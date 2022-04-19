using System.ComponentModel.DataAnnotations;

namespace BlazorSMHI.Shared.Entities
{
    public class MachineLocation
    {
        public Guid Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public float Longitude { get; set; }
        public float Latitude { get; set; }

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