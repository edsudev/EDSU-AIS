using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace EDSU_SYSTEM.Models
{
    public class EuslP
    {
        public int? Id { get; set; }
        public string? Ref { get; set; }
        public string? Type { get; set; }
        public string? PayerName { get; set; }
        public string? Email { get; set; }
        public string? Mode { get; set; }
        public string? ReceiptNo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double? Amount { get; set; }
        public string? Status { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
    }
}
