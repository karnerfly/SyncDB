using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync
{
  public class Analyzer(SourceContext sourceContext, TargetContext targetContext)
  {
    private readonly SourceContext _sourceContext = sourceContext;
    private readonly TargetContext _targetContext = targetContext;

    private async Task<List<TEntity>> GetSourceHashResultsAsync<TEntity>(AnalyzeConfig<TEntity> config)
      where TEntity : class
    {
      var source = config.Source(_sourceContext);
      return await source.ToListAsync();
    }

    private async Task<List<TEntity>> GetTargetHashResultsAsync<TEntity>(AnalyzeConfig<TEntity> config)
      where TEntity : class
    {
      var target = config.Target(_targetContext);
      return await target.ToListAsync();
    }

    public async Task Analyze<TEntity>(AnalyzeConfig<TEntity> config) where TEntity : class
    {
      var sourceTask = GetSourceHashResultsAsync(config);
      var targetTask = GetTargetHashResultsAsync(config);

      await Task.WhenAll(sourceTask, targetTask);

      var sourceResult = sourceTask.Result;
      var targetResult = targetTask.Result;

      PrintSummary(sourceResult, targetResult, config);
    }

    private static void PrintSummary<TEntity>(
    List<TEntity> sourceResult,
    List<TEntity> targetResult,
    AnalyzeConfig<TEntity> config)
    where TEntity : class
    {
      int padding = 40;

      Console.WriteLine("=".PadRight(padding, '='));
      Console.WriteLine($"Table: {typeof(TEntity).Name}");
      Console.WriteLine("=".PadRight(padding, '='));

      Console.WriteLine($"{"Source Records:",-10} {sourceResult.Count}");
      Console.WriteLine($"{"Target Records:",-10} {targetResult.Count}");

      var targetMap = targetResult.ToDictionary(config.IdSelector);
      var targetIds = new HashSet<object>(targetMap.Keys);

      var newRecords = new List<TEntity>();
      var diffRecords = new List<(TEntity Source, TEntity Target)>();

      foreach (var src in sourceResult)
      {
        var id = config.IdSelector(src);
        if (targetMap.TryGetValue(id, out var tgt))
        {
          if (config.DifferFunc != null && config.DifferFunc(src, tgt))
          {
            diffRecords.Add((src, tgt));
          }
        }
        else
        {
          newRecords.Add(src);
        }
      }

      if (diffRecords.Count > 0 && config.PrintChangedRecords != null)
      {
        Console.WriteLine("Records with different values:");
        foreach (var (src, tgt) in diffRecords)
        {
          config.PrintChangedRecords(src, tgt);
        }
      }

      Console.WriteLine("=".PadRight(padding, '='));

      if (newRecords.Count > 0 && config.PrintNewRecords != null)
      {
        Console.WriteLine("New Records:");
        foreach (var r in newRecords)
        {
          config.PrintNewRecords(r);
        }
      }

      Console.WriteLine("=".PadRight(padding, '='));
    }
  }
}
