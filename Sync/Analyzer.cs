using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;

namespace SyncHouseHero.Sync
{
  public class Analyzer(SourceContext sourceContext, TargetContext targetContext)
  {
    public static readonly int Padding = 60;
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

    public async Task Analyze<TEntity>(AnalyzeConfig<TEntity> config, StreamWriter writer) where TEntity : class
    {
      var sourceTask = GetSourceHashResultsAsync(config);
      var targetTask = GetTargetHashResultsAsync(config);

      await Task.WhenAll(sourceTask, targetTask);

      var sourceResult = sourceTask.Result;
      var targetResult = targetTask.Result;

      CreateSummary(sourceResult, targetResult, config, writer);
    }

    private static void CreateSummary<TEntity>(
    List<TEntity> sourceResult,
    List<TEntity> targetResult,
    AnalyzeConfig<TEntity> config,
    StreamWriter writer)
    where TEntity : class
    {
      writer.WriteLine("=".PadRight(Padding, '='));
      writer.WriteLine($"Table: {typeof(TEntity).Name}");
      writer.WriteLine("=".PadRight(Padding, '='));

      writer.WriteLine($"{"Source Records:",-10} {sourceResult.Count}");
      writer.WriteLine($"{"Target Records:",-10} {targetResult.Count}");

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

      if (diffRecords.Count > 0 && config.ActionOnDifferentRecords != null)
      {
        writer.WriteLine($"Records with different values: ({diffRecords.Count})");
        foreach (var (src, tgt) in diffRecords)
        {
          writer.Write(config.ActionOnDifferentRecords(src, tgt));
        }
      }
      else
      {
        writer.WriteLine("All fields are indentical.");
      }

      writer.WriteLine("=".PadRight(Padding, '='));

      if (newRecords.Count > 0 && config.ActionOnNewRecords != null)
      {
        writer.WriteLine($"New Records: ({newRecords.Count})");
        foreach (var r in newRecords)
        {
          writer.Write(config.ActionOnNewRecords(r));
        }
      }
      else
      {
        writer.WriteLine("No new records in source table.");
      }

      writer.WriteLine("=".PadRight(Padding, '='));
      writer.WriteLine();
    }
  }
}
