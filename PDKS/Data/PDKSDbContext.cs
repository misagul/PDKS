using Microsoft.EntityFrameworkCore;
using PDKS.Models;

namespace PDKS.Data
{
    public class PDKSDbContext : DbContext
    {
        public PDKSDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
