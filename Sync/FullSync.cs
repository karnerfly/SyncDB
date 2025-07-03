using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;
using SyncHouseHero.Sync.Config;

namespace SyncHouseHero.Sync
{
  public class FullSync(SourceContext sourceContext, TargetContext targetContext)
  {
    private readonly SourceContext _sourceContext = sourceContext;
    private readonly TargetContext _targetContext = targetContext;

    public async Task StartSync<TEntity>(SyncConfig<TEntity> config) where TEntity : class
    {
      var sourceData = await config.Source(_sourceContext).AsNoTracking().ToListAsync();
      var target = config.Target(_targetContext);

      foreach (var record in sourceData)
      {
        var key = config.SelectKey(record);
        var exists = await target.FindAsync(key);

        if (exists is null)
        {
          target.Add(record);
        }
        else
        {
          target.Entry(exists).CurrentValues.SetValues(record);
        }
      }

      //var deletedRows = target.Except(sourceData).ToList();
      //_targetContext.Set<TEntity>().RemoveRange(deletedRows);

      await _targetContext.SaveChangesAsync();
    }
  }
}
