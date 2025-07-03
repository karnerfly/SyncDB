using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;
using SyncHouseHero.Sync.Config;

namespace SyncHouseHero.Sync
{
  public class Analyzer(SourceContext sourceContext, TargetContext targetContext)
  {
    public static readonly int PrimaryPadding = 60;
    public static readonly int Alignment = -10;
    private readonly SourceContext _sourceContext = sourceContext;
    private readonly TargetContext _targetContext = targetContext;

    private async Task<List<TEntity>> GetSourceRecordsAsync<TEntity>(IAnalyzeConfig<TEntity> config)
      where TEntity : class
    {
      var source = config.Source(_sourceContext);
      return await source.ToListAsync();
    }

    private async Task<List<TEntity>> GetTargetRecordsAsync<TEntity>(IAnalyzeConfig<TEntity> config)
      where TEntity : class
    {
      var target = config.Target(_targetContext);
      return await target.ToListAsync();
    }

    public async Task Analyze<TEntity>(IAnalyzeConfig<TEntity> config, bool onlySummary, StreamWriter writer)
      where TEntity : class
    {
      var sourceTask = GetSourceRecordsAsync(config);
      var targetTask = GetTargetRecordsAsync(config);

      await Task.WhenAll(sourceTask, targetTask);

      var sourceResult = sourceTask.Result;
      var targetResult = targetTask.Result;

      CreateSummary(sourceResult, targetResult, config, onlySummary, writer);
    }

    private static void CreateSummary<TEntity>(
    List<TEntity> sourceResult,
    List<TEntity> targetResult,
    IAnalyzeConfig<TEntity> config,
    bool onlySummary,
    StreamWriter writer)
    where TEntity : class
    {
      writer.WriteLine("=".PadRight(PrimaryPadding, '='));
      writer.WriteLine($"Table: {typeof(TEntity).Name}");
      writer.WriteLine("=".PadRight(PrimaryPadding, '='));

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
          if (config.DifferFunc(src, tgt))
          {
            diffRecords.Add((src, tgt));
          }
        }
        else
        {
          newRecords.Add(src);
        }
      }

      if (diffRecords.Count > 0)
      {
        writer.WriteLine($"Records with different values: ({diffRecords.Count})");
        if (config.ActionOnDifferentRecords is not null && !onlySummary)
          foreach (var (src, tgt) in diffRecords)
          {
            writer.WriteLine("-".PadRight(PrimaryPadding, '-'));
            writer.Write(config.ActionOnDifferentRecords(src, tgt));
          }
      }
      else
      {
        writer.WriteLine("All fields are indentical.");
      }

      writer.WriteLine("-".PadRight(PrimaryPadding, '-'));
      if (newRecords.Count > 0)
      {
        writer.WriteLine($"New Records: ({newRecords.Count})");
        if (config.ActionOnNewRecords is not null && !onlySummary)
          foreach (var r in newRecords)
          {
            writer.WriteLine(config.ActionOnNewRecords(r));
          }
      }
      else
      {
        writer.WriteLine("No new records in source table.");
      }

      writer.WriteLine("=".PadRight(PrimaryPadding, '='));
      writer.WriteLine();
    }
  }
}
