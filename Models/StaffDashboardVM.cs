namespace EDSU_SYSTEM.Models
{
    public class StaffDashboardVM
    {
        public List<UgProgress>? UgProjectStudents { get; set; }
        public List<ConversionProjectProgress>? ConversionProjectStudents { get; set; }
        public List<PgProgress>? PgProjectStudents { get; set; }
        public List<TimeTable>? TimeTables { get; set; }
    }
}
