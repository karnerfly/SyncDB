using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Entities;

namespace SyncHouseHero.Data
{
  public class TargetContext(string connectionString) : DbContext
  {
    private readonly string _connectionString = connectionString;

    // TODO: register master data and mapping data enity
    public DbSet<HouseType> HouseType { get; set; }
    public DbSet<PermissionRecord> PermissionRecord { get; set; }
    public DbSet<RoomTemplate> RoomTemplate { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseNpgsql(_connectionString);
    }
  }
}
