namespace EDSU_SYSTEM.Models
{
    public class VcApplication
    {
        public int? Id { get; set; }
        public string? ApplicantId { get; set; }
        public string? FullName { get; set; }
        public string? ContactAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? DOB { get; set; }
        public States? State { get; set; }
        public Lga? Lga { get; set; }
        public Countries? Nationality { get; set; }
        public DateTime? FirstAppointment { get; set; }
        public DateTime? ToProfessor { get; set; }
        public string? Institution { get; set; }
        public int? Year { get; set; }
        public string? Qualifications { get; set; }
        public string? Position { get; set; }
        public string? AdministrativeExperiences { get; set; }
        public string? ProfessionalExperiences { get; set; }
        public string? CommunicationSkills { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
