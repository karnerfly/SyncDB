using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync
{
  public class AnalyzeBuilder<TEntity>(
    Func<SourceContext, DbSet<TEntity>> source,
    Func<TargetContext, DbSet<TEntity>> target)
    where TEntity : class
  {
    private readonly AnalyzeConfig<TEntity> _config = new(source, target);

    public AnalyzeBuilder<TEntity> WithId(Func<TEntity, object> idSelector)
    {
      _config.IdSelector = idSelector;
      return this;
    }

    public AnalyzeBuilder<TEntity> WithDifferFunc(Func<TEntity, TEntity, bool> differFunc)
    {
      _config.DifferFunc = differFunc;
      return this;
    }

    public AnalyzeBuilder<TEntity> OnDifferentRecords(Action<TEntity, TEntity> action)
    {
      _config.ActionOnChangedRecords = action;
      return this;
    }

    public AnalyzeBuilder<TEntity> OnNewRecords(Action<TEntity> action)
    {
      _config.ActionOnNewRecords = action;
      return this;
    }

    public AnalyzeConfig<TEntity> Build()
    {
      return _config;
    }
  }
}
