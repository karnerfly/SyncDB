using SyncHouseHero.Sync;

namespace SyncHouseHero
{
  public static class Program
  {
    public static async Task Main(string[] args)
    {
      // const string SourceConnString = "Host=localhost;Port=5432;Username=postgres;Password=********;Database=practiceDb;";
      // const string TargetConnString = "Host=localhost;Port=5432;Username=postgres;Password=********;Database=practiceDb-Dest;";
      const string Table = "HouseType";
      const string SourceConnString = "Host=ruth.thehousehero.app;Port=5432;Username=postgres;Password=********;Database=pg-hhodb-local-ruth;";
      const string TargetConnString = "Host=ruth.thehousehero.app;Port=5432;Username=postgres;Password=********;Database=pg-hhodb-dev-ruth;";

      var d = new DiffGenerator(SourceConnString, TargetConnString);
      try
      {
        await d.Analyze(Table);
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e);
      }
    }
  }
}