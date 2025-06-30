using SyncHouseHero.Data;
using SyncHouseHero.Sync;
using System.Reflection;

namespace SyncHouseHero
{
  public static class Program
  {
    public static async Task Main(string[] args)
    {
      //const string SourceConnString = "Host=localhost;Port=5432;Username=postgres;Password=********;Database=practiceDb;";
      //const string TargetConnString = "Host=localhost;Port=5432;Username=postgres;Password=********;Database=practiceDb-Dest;";
      //const string Table = "HouseType";
      const string SourceConnString = "Host=ruth.thehousehero.app;Port=5432;Username=postgres;Password=********;Database=pg-hhodb-local-ruth;";
      const string TargetConnString = "Host=ruth.thehousehero.app;Port=5432;Username=postgres;Password=********;Database=pg-hhodb-dev-ruth;";

      var sourceContext = new SourceContext(SourceConnString);
      var targetContext = new TargetContext(TargetConnString);

      var analyzer = new Analyzer(sourceContext, targetContext);

      try
      {
        await Run(analyzer);
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e);
      }
    }

    public static async Task Run(Analyzer analyzer)
    {
      foreach (var kvp in Mapper.EntityAnalyzeConfigMap)
      {
        var configObj = kvp.Value;
        var configType = configObj.GetType();

        if (configType.IsGenericType && configType.GetGenericTypeDefinition() == typeof(AnalyzeConfig<>))
        {
          var entityType = configType.GetGenericArguments()[0];
          var method = typeof(Analyzer).
            GetMethod(nameof(Analyzer.Analyze), BindingFlags.Public | BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

          await (Task?)method.Invoke(analyzer, [configObj])!;
        }
      }
    }
  }
}