namespace Assignment_1.Models
{
    public class AddActivityViewModel
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; }
        public string Subject { get; set; }
        public string CurrentKnowledge { get; set; }
        public DateTime Date { get; set; }
        public int StudyHours { get; set; }
        public int StudyMinutes { get; set; }
        public int BreakHours { get; set; }
        public int BreakMinutes { get; set; }
    }
}
