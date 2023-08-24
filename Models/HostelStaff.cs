using System.ComponentModel.DataAnnotations.Schema;
using static EDSU_SYSTEM.Models.Enum;

namespace EDSU_SYSTEM.Models
{
    public class HostelStaff
    {
        public int Id { get; set; }
        public Staff? Staffs { get; set; }
        [ForeignKey("Staffs")]
        public int? StaffId { get; set; }
        public Hostel? Hostels { get; set; }
        [ForeignKey("Hostels")]
        public int? HostelId { get; set; }
        public bool IsActive { get; set; }
        public HostelStaffType? AdminType { get; set; }

    }
}
