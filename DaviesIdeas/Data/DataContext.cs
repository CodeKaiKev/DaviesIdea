global using Microsoft.EntityFrameworkCore;
namespace DaviesIdeas.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=DaviesIdeas;Trusted_Connection=true;TrustServerCertificate=True");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Idea> Ideas { get; set; }
    }
}
