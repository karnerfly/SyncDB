using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync
{
  public class SyncConfig<TEntity> where TEntity : class
  {
    public Func<SourceContext, DbSet<TEntity>> Source { get; set; }
    public Func<TargetContext, DbSet<TEntity>> Target { get; set; }
    public Func<TEntity, object> SelectKey { get; set; }
  }
}
