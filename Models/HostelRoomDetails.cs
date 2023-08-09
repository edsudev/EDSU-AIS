using System.ComponentModel.DataAnnotations.Schema;

namespace EDSU_SYSTEM.Models
{
    public class HostelRoomDetails
    {
        public int? Id { get; set; }
        [ForeignKey("Hostels")]
        public int? HostelId { get; set; }
        public Hostel? Hostels { get; set; }
        public int? RoomNo { get; set; }
        public int? BedSpaces { get; set; }
        public int? BedSpacesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
