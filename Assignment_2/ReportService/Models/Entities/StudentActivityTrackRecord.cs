namespace ReportService.Models.Entities
{
    public class StudentActivityTrackRecord
    {

        public Guid Id { get; set; }
        public string StudentId { get; set; }
        public string Subject { get; set; }
        public string CurrentKnowledge { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StudyDuration { get; set; }
        public TimeSpan BreakDuration { get; set; }
        
    }
}
