using Assignment_1.Data;
using Assignment_1.Models;
using Assignment_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment_1.Controllers
{
    public class StudentActivityController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1); // Semaphore for thread safety

        public StudentActivityController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Task.Run(() => GenerateSampleDataAsync()).Wait(); // Run data generation asynchronously
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddActivityViewModel model)
        {
            var studyDuration = new TimeSpan(model.StudyHours, model.StudyMinutes, 0);
            var breakDuration = new TimeSpan(model.BreakHours, model.BreakMinutes, 0);

            var activityRecord = new StudentActivityTrackRecord
            {
                Id = Guid.NewGuid(),
                StudentId = model.StudentId,
                Subject = model.Subject,
                CurrentKnowledge = model.CurrentKnowledge,
                Date = model.Date,
                StudyDuration = studyDuration,
                BreakDuration = breakDuration
            };

            await _semaphoreSlim.WaitAsync(); // Ensure thread safety when accessing the database context
            try
            {
                await _dbContext.StudentActivityTrackRecord.AddAsync(activityRecord);
                await _dbContext.SaveChangesAsync();
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listOfRecords = await _dbContext.StudentActivityTrackRecord.ToListAsync();
            return View(listOfRecords);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var activityRecord = await _dbContext.StudentActivityTrackRecord.FindAsync(id);
            if (activityRecord == null)
            {
                return NotFound();
            }

            var viewModel = new AddActivityViewModel
            {
                Id = activityRecord.Id,
                StudentId = activityRecord.StudentId,
                Subject = activityRecord.Subject,
                CurrentKnowledge = activityRecord.CurrentKnowledge,
                Date = activityRecord.Date,
                StudyHours = activityRecord.StudyDuration.Hours,
                StudyMinutes = activityRecord.StudyDuration.Minutes,
                BreakHours = activityRecord.BreakDuration.Hours,
                BreakMinutes = activityRecord.BreakDuration.Minutes
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, AddActivityViewModel model)
        {
            await _semaphoreSlim.WaitAsync(); // Ensure thread safety when accessing the database context
            try
            {
                var record = await _dbContext.StudentActivityTrackRecord.FindAsync(id);
                if (record == null)
                {
                    return NotFound();
                }

                record.Subject = model.Subject;
                record.CurrentKnowledge = model.CurrentKnowledge;
                record.Date = model.Date;
                record.StudyDuration = new TimeSpan(model.StudyHours, model.StudyMinutes, 0);
                record.BreakDuration = new TimeSpan(model.BreakHours, model.BreakMinutes, 0);

                _dbContext.StudentActivityTrackRecord.Update(record);
                await _dbContext.SaveChangesAsync();
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _semaphoreSlim.WaitAsync(); // Ensure thread safety when accessing the database context
            try
            {
                var activityRecord = await _dbContext.StudentActivityTrackRecord.FindAsync(id);
                if (activityRecord == null)
                {
                    return NotFound();
                }

                _dbContext.StudentActivityTrackRecord.Remove(activityRecord);
                await _dbContext.SaveChangesAsync();
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return RedirectToAction("List");
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
                var daysOffset = random.Next(0, 28); // Random day within the last 4 weeks
                var studyHours = random.Next(0, 4);
                var studyMinutes = random.Next(0, 60);
                var breakHours = random.Next(0, 2);
                var breakMinutes = random.Next(0, 60);

                var activityRecord = new StudentActivityTrackRecord
                {
                    Id = Guid.NewGuid(),
                    StudentId = $"A1101", // Adjusted student ID format
                    Subject = subject,
                    CurrentKnowledge = random.Next(0, 100).ToString(),
                    Date = startDate.AddDays(daysOffset),
                    StudyDuration = new TimeSpan(studyHours, studyMinutes, 0),
                    BreakDuration = new TimeSpan(breakHours, breakMinutes, 0)
                };
                activityRecords.Add(activityRecord);
            }

            await _semaphoreSlim.WaitAsync(); // Ensure thread safety when accessing the database context
            try
            {
                _dbContext.StudentActivityTrackRecord.AddRange(activityRecords);
                await _dbContext.SaveChangesAsync();
            }
            finally
            {
                _semaphoreSlim.Release();
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
            var activityRecords = await _dbContext.StudentActivityTrackRecord.ToListAsync();

            var reportData = activityRecords
                .GroupBy(r => new { r.StudentId, r.Subject, Week = GetIso8601WeekOfYear(r.Date) }) // Include StudentId in grouping
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
                StudentId = data.StudentId, // Assign StudentId
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