using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Models;

namespace SyncHouseHero.Data
{
  public class SourceContext(string connectionString) : DbContext
  {
    private readonly string _connectionString = connectionString;

    // TODO: register master data and mapping data enity
    public DbSet<HouseType> HouseType { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseNpgsql(_connectionString);
    }
  }
}
