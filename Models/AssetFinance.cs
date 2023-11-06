using System.ComponentModel.DataAnnotations.Schema;
using static EDSU_SYSTEM.Models.Enum;

namespace EDSU_SYSTEM.Models
{
    public class AssetFinance
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public string? Phone { get; set; }
        public string? Department { get; set; }
        [ForeignKey("EuslAssests")]
        public int? TypeOfAsset { get; set; }
        public EuslAssests? EuslAssests { get; set; }
        public DateTime DeductionStartDate { get; set; }
        public double AmountRequired { get; set; }
        public int MaxPeriod { get; set; }
        public string? SalaryId { get; set; }
        public double? AmountOfDeposit { get; set; }
        public string? AccountNo { get; set; }
        public string? Bank { get; set; }

        //Guarantor's Info
        public string? GuarantorName { get; set; }
        public string? GuarantorPhone { get; set; }
        public string? GuarantorAddress { get; set; }
        public string? GuarantorDepartment { get; set; }
        public string? GuarantorSalaryId { get; set; }

        public double PriceOfAsset { get; set; }
        public int MaxMonthForDeduction { get; set; }
        public string? DeductionStartMonth { get; set; }
        public string? Comment { get; set; }
        public MainStatus Status { get; set; }
    }
}
