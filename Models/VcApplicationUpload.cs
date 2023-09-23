namespace EDSU_SYSTEM.Models
{
    public class VcApplicationUpload
    {
        public int? Id { get; set; }
        public string? ApplicantId { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
