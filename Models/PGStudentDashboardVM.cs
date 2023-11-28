namespace EDSU_SYSTEM.Models
{
    public class PGStudentDashboardVM
    {
        public PgMainWallet? MainWallet { get; set; }
        public List<PgCourseReg>? Courses { get; set; }
        //public List<TimeTable>? TimeTables { get; set; }
    }
}
