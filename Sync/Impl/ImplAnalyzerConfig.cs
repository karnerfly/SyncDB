using SyncHouseHero.Entities;

namespace SyncHouseHero.Sync.Impl
{
  public class ImplAnalyzerConfig
  {
    public static AnalyzeConfig<HouseType> GetHouseType()
    {
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
      return analyzeBuilder.Build();
    }

    public static AnalyzeConfig<PermissionRecord> GetPermissionRecord()
    {
      var analyzeBuilder = new AnalyzeBuilder<PermissionRecord>(sCtx => sCtx.PermissionRecord,
        tCtx => tCtx.PermissionRecord);
      analyzeBuilder.WithId(e => e.Id);
      analyzeBuilder.WithDifferFunc((a, b) => a.Name != b.Name
      || a.SystemName != b.SystemName
      || a.Category != b.Category);
      analyzeBuilder.OnDifferentRecords((src, tgt) =>
      {
        Console.WriteLine("-".PadRight(40, '-'));
        Console.WriteLine($"ID: {src.Id}");
        Console.WriteLine($"+ Old [ Name: {tgt.Name}, SystemName: {tgt.SystemName} ]");
        Console.WriteLine($"- New [ Name: {src.Name}, SystemName: {src.SystemName} ]");
      });
      analyzeBuilder.OnNewRecords(r =>
      {
        Console.WriteLine($"Id: {r.Id,-10} [ Name: {r.Name}, SystemName: {r.SystemName} ]");
      });

      return analyzeBuilder.Build();
    }
  }
}
