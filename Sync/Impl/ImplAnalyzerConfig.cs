using SyncHouseHero.Entities;
using System.Text;

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
        var sb = new StringBuilder();
        sb.AppendLine($"ID: {src.Id}");
        sb.AppendLine($"+ Old [ Name: {tgt.Name}, Active: {tgt.Active} ]");
        sb.AppendLine($"- New [ Name: {src.Name}, Active: {src.Active} ]");
        return sb.ToString();
      });
      analyzeBuilder.OnNewRecords(r =>
      {
        return $"Id: {r.Id,-10} [ Name: {r.Name}, Active: {r.Active} ]\n";
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
        var sb = new StringBuilder();
        sb.AppendLine($"ID: {src.Id}");
        sb.AppendLine($"+ Old [ Name: {tgt.Name}, SystemName: {tgt.SystemName} ]");
        sb.AppendLine($"- New [ Name: {src.Name}, SystemName: {src.SystemName} ]");
        return sb.ToString();
      });
      analyzeBuilder.OnNewRecords(r =>
      {
        return $"Id: {r.Id,-10} [ Name: {r.Name}, SystemName: {r.SystemName} ]\n";
      });

      return analyzeBuilder.Build();
    }
  }
}
