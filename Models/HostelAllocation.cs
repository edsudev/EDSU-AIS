using System.ComponentModel.DataAnnotations.Schema;

namespace EDSU_SYSTEM.Models
{
    public class HostelAllocation
    {
        public int? Id { get; set; }
        [ForeignKey("Hostels")]
        public int? HostelId { get; set; }
        public Hostel? Hostels { get; set; }
        [ForeignKey("HostelRooms")]
        public int? RoomId { get; set; }
        public HostelRoomDetails? HostelRooms { get; set; }

        [ForeignKey("UgMainWallets")]
        public int? WalletId { get; set; }
        public UgMainWallet? UgMainWallets { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
