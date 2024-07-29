using Assignment_1.Data;
using Assignment_1.Models;
using Assignment_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
 

namespace Assignment_1.Controllers
{
    public class StudentActivityController : Controller
    {
        private static List<StudentActivityTrackRecord> activityRecords = new List<StudentActivityTrackRecord>();

        private readonly ApplicationDbContext dbContext;

        public StudentActivityController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            GenerateSampleData();


        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public  async Task<IActionResult>  Add(AddActivityViewModel model)
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

            //activityRecords.Add(activityRecord);

            await dbContext.StudentActivityTrackRecord.AddAsync(activityRecord);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult>  List()

        {
            var listOfRecords = await dbContext.StudentActivityTrackRecord.ToListAsync();
            return View(listOfRecords);
          //  return View(activityRecords);
        }





        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //var activityRecord = activityRecords.FirstOrDefault(x => x.Id == id);
            //if (activityRecord == null)
            //{
            //    return NotFound();
            //}
            var activityRecord = await dbContext.StudentActivityTrackRecord.FindAsync(id);
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
            //var activityRecord = activityRecords.FirstOrDefault(x => x.Id == id);
            //if (activityRecord == null)
            //{
            //    return NotFound();
            //}

            //activityRecord.Subject = model.Subject;
            //activityRecord.CurrentKnowledge = model.CurrentKnowledge;
            //activityRecord.Date = model.Date;
            //activityRecord.StudyDuration = new TimeSpan(model.StudyHours, model.StudyMinutes, 0);
            //activityRecord.BreakDuration = new TimeSpan(model.BreakHours, model.BreakMinutes, 0);

            var record = await dbContext.StudentActivityTrackRecord.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            record.Subject = model.Subject;
            record.CurrentKnowledge = model.CurrentKnowledge;
            record.Date = model.Date;
            record.StudyDuration = new TimeSpan(model.StudyHours, model.StudyMinutes, 0);
            record.BreakDuration = new TimeSpan(model.BreakHours, model.BreakMinutes, 0);

            dbContext.StudentActivityTrackRecord.Update(record);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            //var activityRecord = activityRecords.FirstOrDefault(x => x.Id == id);
            //if (activityRecord != null)
            //{
            //    activityRecords.Remove(activityRecord);
            //}

            //return RedirectToAction("List");

            var activityRecord = await dbContext.StudentActivityTrackRecord.FindAsync(id);
            if (activityRecord == null)
            {
                return NotFound();
            }

            dbContext.StudentActivityTrackRecord.Remove(activityRecord);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }









        // Generate sample data for testing
        private void GenerateSampleData()
        {
            var random = new Random();
            var subjects = new List<string> { "Math", "Science", "History", "English", "Art", "Physics", "Chemistry", "Biology" };
            var startDate = DateTime.Now.AddDays(-28); // Start from 4 weeks ago

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
                dbContext.StudentActivityTrackRecord.Add(activityRecord);
                dbContext.SaveChanges();

                //activityRecords.Add(activityRecord);
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

        //[HttpGet]
        //public IActionResult Report()
        //{
        //    //GenerateSampleData(); // Ensure this function populates the activityRecords list

        //    var reportData = activityRecords
        //        .GroupBy(r => new { r.StudentId, r.Subject, Week = GetIso8601WeekOfYear(r.Date) }) // Include StudentId in grouping
        //        .Select(g => new
        //        {
        //            StudentId = g.Key.StudentId, // Include StudentId in the anonymous type
        //            Subject = g.Key.Subject,
        //            Week = g.Key.Week,
        //            DailyStudyTime = g.GroupBy(r => r.Date.Date).Select(dayGroup => new DailyActivity
        //            {
        //                Date = dayGroup.Key,
        //                DailyStudyTime = Math.Round(dayGroup.Sum(r => r.StudyDuration.TotalHours), 2),
        //                DailyBreakTime = Math.Round(dayGroup.Sum(r => r.BreakDuration.TotalHours), 2)
        //            }).ToList(),
        //            TotalStudyTimeForWeek = Math.Round(g.Sum(r => r.StudyDuration.TotalHours), 2),
        //            TotalBreakTimeForWeek = Math.Round(g.Sum(r => r.BreakDuration.TotalHours), 2)
        //        })
        //        .ToList();

        //    var reportViewModel = reportData.Select((data, index) => new WeeklyReportViewModel
        //    {
        //        StudentId = data.StudentId, // Assign StudentId
        //        Subject = data.Subject,
        //        WeekStartAndEndDate = $"{GetWeekStartDate(data.Week).ToShortDateString()} - {GetWeekEndDate(data.Week).ToShortDateString()}",
        //        DailyActivities = data.DailyStudyTime,
        //        MovingAverageStudyTime = data.DailyStudyTime.Any() ? Math.Round(data.DailyStudyTime.Average(day => day.DailyStudyTime), 2) : 0,
        //        MovingAverageBreakTime = data.DailyStudyTime.Any() ? Math.Round(data.DailyStudyTime.Average(day => day.DailyBreakTime), 2) : 0,
        //        TotalStudyTimeForCurrentWeek = data.TotalStudyTimeForWeek,
        //        TotalBreakTimeForCurrentWeek = data.TotalBreakTimeForWeek,
        //        GradeForThisWeek = GetGrade(data.TotalStudyTimeForWeek),
        //        GradeForUpcomingWeek = GetGrade(data.TotalStudyTimeForWeek - 1.5)
        //    }).ToList();

        //    return View(reportViewModel);
        //}


        // real one : dont delete
        //[HttpGet]
        //public async Task<IActionResult> Report()
        //{
            
        //    var activityRecords = await dbContext.StudentActivityTrackRecord.ToListAsync();

        //    var reportData = activityRecords
        //        .GroupBy(r => new { r.StudentId, r.Subject, Week = GetIso8601WeekOfYear(r.Date) }) // Include StudentId in grouping
        //        .Select(g => new
        //        {
        //            StudentId = g.Key.StudentId, 
        //            Subject = g.Key.Subject,
        //            Week = g.Key.Week,
        //            DailyStudyTime = g.GroupBy(r => r.Date.Date).Select(dayGroup => new DailyActivity
        //            {
        //                Date = dayGroup.Key,
        //                DailyStudyTime = Math.Round(dayGroup.Sum(r => r.StudyDuration.TotalHours), 2),
        //                DailyBreakTime = Math.Round(dayGroup.Sum(r => r.BreakDuration.TotalHours), 2)
        //            }).ToList(),
        //            TotalStudyTimeForWeek = Math.Round(g.Sum(r => r.StudyDuration.TotalHours), 2),
        //            TotalBreakTimeForWeek = Math.Round(g.Sum(r => r.BreakDuration.TotalHours), 2)
        //        })
        //        .ToList();

        //    var reportViewModel = reportData.Select((data, index) => new WeeklyReportViewModel
        //    {
        //        StudentId = data.StudentId, // Assign StudentId
        //        Subject = data.Subject,
        //        WeekStartAndEndDate = $"{GetWeekStartDate(data.Week).ToShortDateString()} - {GetWeekEndDate(data.Week).ToShortDateString()}",
        //        DailyActivities = data.DailyStudyTime,
        //        MovingAverageStudyTime = data.DailyStudyTime.Any() ? Math.Round(data.DailyStudyTime.Average(day => day.DailyStudyTime), 2) : 0,
        //        MovingAverageBreakTime = data.DailyStudyTime.Any() ? Math.Round(data.DailyStudyTime.Average(day => day.DailyBreakTime), 2) : 0,
        //        TotalStudyTimeForCurrentWeek = data.TotalStudyTimeForWeek,
        //        TotalBreakTimeForCurrentWeek = data.TotalBreakTimeForWeek,
        //        GradeForThisWeek = GetGrade(data.TotalStudyTimeForWeek),
        //        GradeForUpcomingWeek = GetGrade(data.TotalStudyTimeForWeek - 1.5)
        //    }).ToList();

        //    return View(reportViewModel);
        //}



    }
}
