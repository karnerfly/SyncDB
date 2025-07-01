using SyncHouseHero.Data;
using SyncHouseHero.Sync;
using System.Reflection;
using Cocona;

namespace SyncHouseHero
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      CoconaApp.Run(async ([Option(Description = "connection url for source database")] string Source,
        [Option(Description = "connection url for target database")] string Target,
        [Option(Description = "file path that contains table names")] string File,
        [Option(Description = "output file path")] string Out) =>
      {
        try
        {
          var sourceContext = new SourceContext(Source);
          var targetContext = new TargetContext(Target);
          TestSourceTargetConnection.Test(sourceContext, targetContext);

          var analyzer = new Analyzer(sourceContext, targetContext);

          using var inputFile = new StreamReader(File);
          using var outputFile = new StreamWriter(Out);

          string? tableName = inputFile.ReadLine();
          Console.WriteLine("Analyzing....");
          while (tableName is not null)
          {
            if (!Mapper.EntityAnalyzeConfigMap.TryGetValue(tableName, out var config))
            {
              throw new Exception($"table {tableName} not found");
            }

            Console.WriteLine($"Table: {tableName}");
            await Run(analyzer, config, outputFile);
            tableName = inputFile.ReadLine();
          }
          Console.WriteLine("Done.");
        }
        catch (Exception e)
        {
          Console.Error.WriteLine(e);
        }
      });
    }

    public static async Task Run(Analyzer analyzer, object config, StreamWriter sw)
    {
      var configType = config.GetType();

      if (configType.IsGenericType && configType.GetGenericTypeDefinition() == typeof(AnalyzeConfig<>))
      {
        var entityType = configType.GetGenericArguments()[0];
        var method = typeof(Analyzer).
          GetMethod(nameof(Analyzer.Analyze), BindingFlags.Public | BindingFlags.Instance)!
          .MakeGenericMethod(entityType);

        await (Task)method.Invoke(analyzer, [config, sw])!;
      }
    }
  }
}