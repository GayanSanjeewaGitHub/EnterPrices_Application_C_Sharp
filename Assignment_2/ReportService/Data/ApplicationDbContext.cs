using ReportService.Models.Entities;
using Microsoft.EntityFrameworkCore;
 

namespace ReportService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<StudentActivityTrackRecord> StudentActivityTrackRecord  { get; set;}
}

}
