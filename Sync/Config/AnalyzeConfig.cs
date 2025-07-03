using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync.Config
{
  public class AnalyzeConfig<TEntity> : IAnalyzeConfig<TEntity>
    where TEntity : class
  {
    public Func<SourceContext, DbSet<TEntity>> source;
    public Func<TargetContext, DbSet<TEntity>> target;
    public Func<TEntity, object> idSelector;
    public Func<TEntity, TEntity, bool> differFunc;
    public Func<TEntity, TEntity, string>? actionOnDifferentRecords;
    public Func<TEntity, string>? actionOnNewRecords;

    public Func<SourceContext, DbSet<TEntity>> Source { get => source; }
    public Func<TargetContext, DbSet<TEntity>> Target { get => target; }
    public Func<TEntity, object> IdSelector { get => idSelector; }
    public Func<TEntity, TEntity, bool> DifferFunc { get => differFunc; }
    public Func<TEntity, TEntity, string>? ActionOnDifferentRecords { get => actionOnDifferentRecords; }
    public Func<TEntity, string>? ActionOnNewRecords { get => actionOnNewRecords; }

    private AnalyzeConfig() { }

    public class Builder
    {
      private readonly AnalyzeConfig<TEntity> _config;

      public Builder(
        Func<SourceContext, DbSet<TEntity>> source,
        Func<TargetContext, DbSet<TEntity>> target)
      {
        _config = new()
        {
          source = source,
          target = target
        };
      }

      public Builder WithId(Func<TEntity, object> idSelector)
      {
        _config.idSelector = idSelector;
        return this;
      }

      public Builder WithDifferFunc(Func<TEntity, TEntity, bool> differFunc)
      {
        _config.differFunc = differFunc;
        return this;
      }

      public Builder OnDifferentRecords(Func<TEntity, TEntity, string> action)
      {
        _config.actionOnDifferentRecords = action;
        return this;
      }

      public Builder OnNewRecords(Func<TEntity, string> action)
      {
        _config.actionOnNewRecords = action;
        return this;
      }

      public IAnalyzeConfig<TEntity> Build()
      {
        return _config;
      }
    }
  }
}
