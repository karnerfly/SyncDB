using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync.Config
{
  public interface IAnalyzeConfig<TEntity> where TEntity : class
  {
    Func<SourceContext, DbSet<TEntity>> Source { get; }
    public Func<TargetContext, DbSet<TEntity>> Target { get; }
    public Func<TEntity, object> IdSelector { get; }
    public Func<TEntity, TEntity, bool> DifferFunc { get; }
    public Func<TEntity, TEntity, string>? ActionOnDifferentRecords { get; }
    public Func<TEntity, string>? ActionOnNewRecords { get; }
  }
}
