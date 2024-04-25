using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FPTJob_1670.Models;

namespace FPTJob_1670.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FPTJob_1670.Models.Seeker> Seeker { get; set; } = default!;
        public DbSet<FPTJob_1670.Models.Employer> Employer { get; set; } = default!;
        public DbSet<FPTJob_1670.Models.Job> Job { get; set; } = default!;
        public DbSet<FPTJob_1670.Models.Application> Application { get; set; } = default!;

        public DbSet<Notification> Notification { get; set; } = default!;

        public DbSet<FPTJob_1670.Models.Message> Message { get; set; } = default!;
    }
}