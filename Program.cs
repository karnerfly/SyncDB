using SyncHouseHero.Data;
using SyncHouseHero.Models;
using SyncHouseHero.Sync;

namespace SyncHouseHero
{
  public static class Program
  {
    public static async Task Main(string[] args)
    {
      //const string SourceConnString = "Host=localhost;Port=5432;Username=postgres;Password=ajay9339;Database=practiceDb;";
      //const string TargetConnString = "Host=localhost;Port=5432;Username=postgres;Password=ajay9339;Database=practiceDb-Dest;";
      //const string Table = "HouseType";
      const string SourceConnString = "Host=ruth.thehousehero.app;Port=5432;Username=postgres;Password=Sealdah700009;Database=pg-hhodb-local-ruth;";
      const string TargetConnString = "Host=ruth.thehousehero.app;Port=5432;Username=postgres;Password=Sealdah700009;Database=pg-hhodb-dev-ruth;";

      var sourceContext = new SourceContext(SourceConnString);
      var targetContext = new TargetContext(TargetConnString);

      var analyzeBuilder = new AnalyzeBuilder<HouseType>(sCtx => sCtx.HouseType, tCtx => tCtx.HouseType);
      analyzeBuilder.WithId(e => e.Id);
      analyzeBuilder.WithDifferFunc((a, b) => a.Name != b.Name || a.Active != b.Active);
      analyzeBuilder.OnDifferentRecords((src, tgt) =>
      {
        Console.WriteLine("-".PadRight(40, '-'));
        Console.WriteLine($"ID: {src.Id}");
        Console.WriteLine($"+ Old [ Name: {tgt.Name}, Active: {tgt.Active} ]");
        Console.WriteLine($"- New [ Name: {src.Name}, Active: {src.Active} ]");
      });
      analyzeBuilder.OnNewRecords(r =>
      {
        Console.WriteLine($"Id: {r.Id,-10} [ Name: {r.Name}, Active: {r.Active} ]");
      });

      var config = analyzeBuilder.Build();
      var analyzer = new Analyzer(sourceContext, targetContext);

      try
      {

        await analyzer.Analyze(config);

        //var s = new FullSync(sourceContext, targetContext);
        //var config = new Config<Subject>
        //{
        //  Source = ctx => ctx.Subjects,
        //  Target = ctx => ctx.Subjects,
        //  SelectKey = r => r.Id
        //};

        //await s.StartSync(config);
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e);
      }
    }
  }
}