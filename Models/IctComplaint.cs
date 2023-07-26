﻿using static EDSU_SYSTEM.Models.Enum;
namespace EDSU_SYSTEM.Models

{
    public class IctComplaint
    {
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public string? Dept { get; set; }
        public string? CC { get; set; }
        public string? Location { get; set; }
        public string? Complain { get; set; }
        //Response by works Dept
        public string? Response { get; set; }
        //By complainant
        public WorksStatusAcknowledgment? Acknowledgment { get; set; }
        public WorksStatus? Status { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
