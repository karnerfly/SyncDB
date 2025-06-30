using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync
{
  public class AnalyzeConfig<TEntity>(
    Func<SourceContext, DbSet<TEntity>> source,
    Func<TargetContext, DbSet<TEntity>> target)
    where TEntity : class
  {
    public Func<SourceContext, DbSet<TEntity>> Source { get; set; } = source;
    public Func<TargetContext, DbSet<TEntity>> Target { get; set; } = target;
    public Func<TEntity, object> IdSelector { get; set; }
    public Func<TEntity, TEntity, bool>? DifferFunc { get; set; }
    public Action<TEntity, TEntity>? ActionOnChangedRecords { get; set; }
    public Action<TEntity>? ActionOnNewRecords { get; set; }
  }
}
