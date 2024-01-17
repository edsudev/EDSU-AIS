using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDSU_SYSTEM.Models
{
    public class HostelUpgrade
    {
        public int Id { get; set; }
        [ForeignKey("PreviousHostels")]
        public int From { get; set; }
        [ForeignKey("NewHostels")]
        public int To { get; set; }
        public Hostel? PreviousHostels { get; set; }
        public Hostel? NewHostels { get; set; }
    }
}
