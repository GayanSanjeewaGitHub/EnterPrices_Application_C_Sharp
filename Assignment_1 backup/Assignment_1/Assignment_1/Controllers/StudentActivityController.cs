using Assignment_1.Models;
using Assignment_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_1.Controllers
{
    public class StudentActivityController : Controller
    {
        private static List<StudentActivityTrackRecord> activityRecords = new List<StudentActivityTrackRecord>();

     

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(AddActivityViewModel model)
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

            activityRecords.Add(activityRecord);

            return RedirectToAction("Add");
        }

        [HttpGet]
        public IActionResult List()
        {
            return View(activityRecords);
        }





        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var activityRecord = activityRecords.FirstOrDefault(x => x.Id == id);
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
        public IActionResult Edit(Guid id, AddActivityViewModel model)
        {
            var activityRecord = activityRecords.FirstOrDefault(x => x.Id == id);
            if (activityRecord == null)
            {
                return NotFound();
            }

            activityRecord.Subject = model.Subject;
            activityRecord.CurrentKnowledge = model.CurrentKnowledge;
            activityRecord.Date = model.Date;
            activityRecord.StudyDuration = new TimeSpan(model.StudyHours, model.StudyMinutes, 0);
            activityRecord.BreakDuration = new TimeSpan(model.BreakHours, model.BreakMinutes, 0);

            return RedirectToAction("List");
        }


        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var activityRecord = activityRecords.FirstOrDefault(x => x.Id == id);
            if (activityRecord != null)
            {
                activityRecords.Remove(activityRecord);
            }

            return RedirectToAction("List");
        }









        // Generate sample data for testing
        private void GenerateSampleData()
        {
            var random = new Random();
            var subjects = new List<string> { "Math", "Science", "History", "English", "Art", "Physics", "Chemistry", "Biology" };
            var startDate = DateTime.Now.AddDays(-28); // Start from 4 weeks ago

            for (int i = 0; i < 100; i++)
            {
                var subject = subjects[random.Next(subjects.Count)];
                var daysOffset = random.Next(0, 28); // Random day within the last 4 weeks
                var studyHours = random.Next(0, 4);
                var studyMinutes = random.Next(0, 60);
                var breakHours = random.Next(0, 2);
                var breakMinutes = random.Next(0, 60);

                var record = new StudentActivityTrackRecord
                {
                    Id = Guid.NewGuid(),
                    StudentId = $"A{i + 1}", // Adjusted student ID format
                    Subject = subject,
                    CurrentKnowledge = random.Next(0, 100).ToString(),
                    Date = startDate.AddDays(daysOffset),
                    StudyDuration = new TimeSpan(studyHours, studyMinutes, 0),
                    BreakDuration = new TimeSpan(breakHours, breakMinutes, 0)
                };

                activityRecords.Add(record);
            }
        }

        // Get ISO 8601 week of year
        private static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        // Get start date of the week
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
            var result = firstMonday.AddDays(weekNum * 7);
            return result;
        }

        // Get end date of the week
        private DateTime GetWeekEndDate(int weekOfYear)
        {
            return GetWeekStartDate(weekOfYear).AddDays(6);
        }

        // Get grade based on total study hours
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

        // GET: /StudentActivity/Report
        [HttpGet]
        public IActionResult Report()
        {
            GenerateSampleData();

            var reportData = activityRecords
                .GroupBy(r => new { r.Subject, Week = GetIso8601WeekOfYear(r.Date) })
                .Select(g => new
                {
                    Subject = g.Key.Subject,
                    Week = g.Key.Week,
                    DailyStudyTime = g.GroupBy(r => r.Date.Date).Select(dayGroup => new
                    {
                        Date = dayGroup.Key,
                        StudyTime = dayGroup.Sum(r => r.StudyDuration.TotalHours),
                        BreakTime = dayGroup.Sum(r => r.BreakDuration.TotalHours)
                    }).ToList(),
                    TotalStudyTimeForWeek = g.Sum(r => r.StudyDuration.TotalHours),
                    TotalBreakTimeForWeek = g.Sum(r => r.BreakDuration.TotalHours)
                })
                .ToList();

            var reportViewModel = reportData.Select((data, index) => new WeeklyReportViewModel
            {
                StudentId = $"A{index + 1}",
                Subject = data.Subject,
                WeekStartAndEndDate = $"{GetWeekStartDate(data.Week).ToShortDateString()} - {GetWeekEndDate(data.Week).ToShortDateString()}",
                DailyStudyTime = data.DailyStudyTime.Any() ? data.DailyStudyTime.Sum(day => day.StudyTime) : 0,
                DailyBreakTime = data.DailyStudyTime.Any() ? data.DailyStudyTime.Sum(day => day.BreakTime) : 0,
                MovingAverageStudyTime = data.DailyStudyTime.Any() ? data.DailyStudyTime.Average(day => day.StudyTime) : 0,
                MovingAverageBreakTime = data.DailyStudyTime.Any() ? data.DailyStudyTime.Average(day => day.BreakTime) : 0,
                TotalStudyTimeForCurrentWeek = data.TotalStudyTimeForWeek,
                TotalBreakTimeForCurrentWeek = data.TotalBreakTimeForWeek,
                GradeForThisWeek = GetGrade(data.TotalStudyTimeForWeek),
                GradeForUpcomingWeek = GetGrade(data.TotalStudyTimeForWeek - 1.5)
            }).ToList();

            return View(reportViewModel);
        }
    }
}
