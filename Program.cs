using SyncHouseHero.Data;
using SyncHouseHero.Sync;
using System.Reflection;
using Cocona;
using Microsoft.Extensions.Configuration;
using SyncHouseHero.Sync.Config;

namespace SyncHouseHero
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      var config = GetConfiguration();
      var OutputDirectory = "output";
      var OutputFilePath = OutputDirectory + "\\" + "Summary_" + DateTime.UtcNow.Ticks + ".txt";

      if (!Directory.Exists(OutputDirectory))
      {
        Directory.CreateDirectory(OutputDirectory);
      }

      var sourceConnectionString = config.GetConnectionString("SourceString");
      var targetConnectionString = config.GetConnectionString("TargetString");

      if (sourceConnectionString is null || targetConnectionString is null)
      {
        throw new Exception("source or target string is missing in configuration json file");
      }

      CoconaApp.Run(
        async (
        [Option(Description = "file path that contains table names")] string tables,
        [Option(Description = "set to generate only summary")] bool onlySummary) =>
      {
        try
        {
          var sourceContext = new SourceContext(sourceConnectionString);
          var targetContext = new TargetContext(targetConnectionString);
          TestSourceTargetConnection.Test(sourceContext, targetContext);

          using var inputFile = new StreamReader(tables);
          using var outputFile = new StreamWriter(OutputFilePath);

          var analyzer = new Analyzer(sourceContext, targetContext);

          Console.WriteLine("Analyzing....");
          var tableName = inputFile.ReadLine();
          while (tableName is not null)
          {
            if (!Mapper.EntityAnalyzeConfigMap.TryGetValue(tableName, out var config))
            {
              throw new Exception($"table {tableName} not found");
            }

            Console.WriteLine($"Table: {tableName}");
            await Run(analyzer, config, onlySummary, outputFile);
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

    public static async Task Run(Analyzer analyzer, object config, bool onlySummary, StreamWriter sw)
    {
      var type = config.GetType();
      var iface = type.GetInterfaces().FirstOrDefault(i =>
      i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAnalyzeConfig<>))
        ?? throw new Exception("invalid analyze config type");

      var entityType = type.GetGenericArguments()[0];
      var method = typeof(Analyzer).
        GetMethod(nameof(Analyzer.Analyze), BindingFlags.Public | BindingFlags.Instance)!
        .MakeGenericMethod(entityType);

      await (Task)method.Invoke(analyzer, [config, onlySummary, sw])!;
    }

    public static IConfiguration GetConfiguration()
    {
      return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
  }
}