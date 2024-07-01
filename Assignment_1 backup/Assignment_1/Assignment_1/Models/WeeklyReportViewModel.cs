namespace Assignment_1.Models
{
    public class WeeklyReportViewModel
    {
        public string StudentId { get; set; }
        public string Subject { get; set; }
        public string WeekStartAndEndDate { get; set; }
        public DateTime Date { get; set; }
        public double DailyStudyTime { get; set; }
        public double DailyBreakTime { get; set; }
        public double MovingAverageStudyTime { get; set; }
        public double MovingAverageBreakTime { get; set; }
        public double TotalStudyTimeForCurrentWeek { get; set; }
        public double TotalBreakTimeForCurrentWeek { get; set; }
        public string GradeForThisWeek { get; set; }
        public string GradeForUpcomingWeek { get; set; }
    }
}
