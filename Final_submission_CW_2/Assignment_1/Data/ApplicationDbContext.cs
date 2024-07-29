using Assignment_1.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment_1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<StudentActivityTrackRecord> StudentActivityTrackRecord  { get; set;}
}

}
