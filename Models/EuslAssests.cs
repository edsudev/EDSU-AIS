using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
namespace EDSU_SYSTEM.Models
{
    public class EuslAssests
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
    }
}
