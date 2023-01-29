using Microsoft.EntityFrameworkCore;

namespace GlobalServer.DAL
{

    public class GlobalServerDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=IOTHomeSecurityGlobal;Integrated Security=True;");
            }
        }
    }
}