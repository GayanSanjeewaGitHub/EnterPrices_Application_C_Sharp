using ReportService.Data;
using ReportService.Models;
using ReportService.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Controllers
{
    public class StudentActivityController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1); 

        public StudentActivityController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Task.Run(() => GenerateSampleDataAsync()).Wait();  
        }
 
        private async Task GenerateSampleDataAsync()
        {
            var random = new Random();
            var subjects = new List<string> { "Math", "Science", "History", "English", "Art", "Physics", "Chemistry", "Biology" };
            var startDate = DateTime.Now.AddDays(-28); // Start from 4 weeks ago

            var activityRecords = new List<StudentActivityTrackRecord>();

            for (int i = 0; i < 10; i++)
            {
                var subject = subjects[random.Next(subjects.Count)];
                var daysOffset = random.Next(0, 28);  
                var studyHours = random.Next(0, 4);
                var studyMinutes = random.Next(0, 60);
                var breakHours = random.Next(0, 2);
                var breakMinutes = random.Next(0, 60);

                var activityRecord = new StudentActivityTrackRecord
                {
                    Id = Guid.NewGuid(),
                    StudentId = $"A1101",  
                    Subject = subject,
                    CurrentKnowledge = random.Next(0, 100).ToString(),
                    Date = startDate.AddDays(daysOffset),
                    StudyDuration = new TimeSpan(studyHours, studyMinutes, 0),
                    BreakDuration = new TimeSpan(breakHours, breakMinutes, 0)
                };
                activityRecords.Add(activityRecord);
            }

            await semaphoreSlim.WaitAsync();  
            try
            {
                dbContext.StudentActivityTrackRecord.AddRange(activityRecords);
                await dbContext.SaveChangesAsync();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private DateTime GetWeekStartDate(int weekOfYear)
        {
            var jan1 = new DateTime(DateTime.Now.Year, 1, 1);
            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            var firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            return firstMonday.AddDays(weekNum * 7);
        }

        private DateTime GetWeekEndDate(int weekOfYear)
        {
            return GetWeekStartDate(weekOfYear).AddDays(6);
        }

        private string GetGrade(double totalStudyHours)
        {
            if (totalStudyHours > 10)
                return "A";
            if (totalStudyHours > 8)
                return "B";
            if (totalStudyHours > 5)
                return "C";
            return "W";
        }

        [HttpGet]
        public async Task<IActionResult> Report()
        {
            var activityRecords = await dbContext.StudentActivityTrackRecord.ToListAsync();

            var reportData = activityRecords
                .GroupBy(r => new { r.StudentId, r.Subject, Week = GetIso8601WeekOfYear(r.Date) })
                .Select(g => new
                {
                    StudentId = g.Key.StudentId,
                    Subject = g.Key.Subject,
                    Week = g.Key.Week,
                    DailyStudyTime = g.GroupBy(r => r.Date.Date).Select(dayGroup => new DailyActivity
                    {
                        Date = dayGroup.Key,
                        DailyStudyTime = Math.Round(dayGroup.Sum(r => r.StudyDuration.TotalHours), 2),
                        DailyBreakTime = Math.Round(dayGroup.Sum(r => r.BreakDuration.TotalHours), 2)
                    }).ToList(),
                    TotalStudyTimeForWeek = Math.Round(g.Sum(r => r.StudyDuration.TotalHours), 2),
                    TotalBreakTimeForWeek = Math.Round(g.Sum(r => r.BreakDuration.TotalHours), 2)
                })
                .ToList();

            var reportViewModel = reportData.Select((data, index) => new WeeklyReportViewModel
            {
                StudentId = data.StudentId,
                Subject = data.Subject,
                WeekStartAndEndDate = $"{GetWeekStartDate(data.Week).ToShortDateString()} - {GetWeekEndDate(data.Week).ToShortDateString()}",
                DailyActivities = data.DailyStudyTime,
                MovingAverageStudyTime = data.DailyStudyTime.Any() ? Math.Round(data.DailyStudyTime.Average(day => day.DailyStudyTime), 2) : 0,
                MovingAverageBreakTime = data.DailyStudyTime.Any() ? Math.Round(data.DailyStudyTime.Average(day => day.DailyBreakTime), 2) : 0,
                TotalStudyTimeForCurrentWeek = data.TotalStudyTimeForWeek,
                TotalBreakTimeForCurrentWeek = data.TotalBreakTimeForWeek,
                GradeForThisWeek = GetGrade(data.TotalStudyTimeForWeek),
                GradeForUpcomingWeek = GetGrade(data.TotalStudyTimeForWeek - 1.5)
            }).ToList();

            return View(reportViewModel);
        }
    }
}