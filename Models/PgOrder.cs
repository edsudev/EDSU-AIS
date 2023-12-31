﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDSU_SYSTEM.Models
{
    public class PgOrder
    {
        public int? Id { get; set; }
        public string? Ref { get; set; }
        public string? Email { get; set; }
        public string? Mode { get; set; }
        public string? ReceiptNo { get; set; }
        [ForeignKey("Wallets")]
        public int? WalletId { get; set; }
        public PgSubWallet? Wallets { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double? Amount { get; set; }
        //Always credit
        public string? Type { get; set; }
        [ForeignKey("OtherFees")]
        public int? OtherFeesDesc { get; set; }
        public OtherFees? OtherFees { get; set; }
        public string? Status { get; set; }
        public Session? Sessions { get; set; }
        [ForeignKey("Sessions")]
        public int? SessionId { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        
    }
}
