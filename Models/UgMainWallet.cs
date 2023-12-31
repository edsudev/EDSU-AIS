﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using static EDSU_SYSTEM.Models.Enum;

namespace EDSU_SYSTEM.Models
{
    //This is class for all wallets
    public class UgMainWallet
    {
        public int? Id { get; set; }
        public string? WalletId { get; set; }
        public int? ApplicantId { get; set; }
        public StudentType? StudentType { get; set; }
        public string? Name { get; set; }
        public string? UTME { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CreditBalance { get; set; }
        //Debit stores how much a student is meant to pay
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BulkDebitBalanace { get; set; }
        public bool? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
       
    }
}
