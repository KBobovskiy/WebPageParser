using DataBaseContext.Model;
using Microsoft.EntityFrameworkCore;

namespace DataBaseContext
{
    public class SqliteContext : DbContext
    {
        private readonly string _connectionString;

        public SqliteContext(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DbSet<OzonProduct> OzonProducts { get; set; }

        public DbSet<OzonProductPriceHistory> OzonProductPriceHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}