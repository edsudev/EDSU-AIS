using System.ComponentModel.DataAnnotations.Schema;

namespace EDSU_SYSTEM.Models
{
    public class HostelAllocation
    {
        public int? Id { get; set; }
        [ForeignKey("Hostels")]
        public int? HostelId { get; set; }
        public int? RoomIdId { get; set; }
        public Hostel? Hostels { get; set; }
        [ForeignKey("Students")]
        public int? StudentId { get; set; }
        public Student? Students { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
