using System.ComponentModel.DataAnnotations.Schema;

namespace EDSU_SYSTEM.Models
{
    public class HostelReceipt
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        [ForeignKey("Hostels")]
        public int? HostelId { get; set; }
        public Hostel? Hostels { get; set; }
        public string? TransactionID { get; set; }
        public bool? HasRoom { get; set; }
    }
}
