using SyncHouseHero.Entities;
using SyncHouseHero.Sync.Config;
using System.Text;

namespace SyncHouseHero.Sync.Impl
{
  public class ImplAnalyzeConfig
  {
    public static IAnalyzeConfig<HouseType> GetHouseType()
    {
      var builder = new AnalyzeConfig<HouseType>.Builder(sCtx => sCtx.HouseType, tCtx => tCtx.HouseType);
      builder.WithId(e => e.Id);
      builder.WithDifferFunc((a, b) => !a.Identical(b));
      builder.OnDifferentRecords((src, tgt) =>
      {
        var sb = new StringBuilder();
        sb.AppendLine($"ID: {src.Id}");
        sb.AppendLine($"+ Old [ Name: {tgt.Name}, Active: {tgt.Active} ]");
        sb.AppendLine($"- New [ Name: {src.Name}, Active: {src.Active} ]");
        return sb.ToString();
      });
      builder.OnNewRecords(r =>
      {
        return $"Id: {r.Id,-10} [ Name: {r.Name}, Active: {r.Active} ]";
      });
      return builder.Build();
    }

    public static IAnalyzeConfig<PermissionRecord> GetPermissionRecord()
    {
      var builder = new AnalyzeConfig<PermissionRecord>.Builder(sCtx => sCtx.PermissionRecord,
        tCtx => tCtx.PermissionRecord);
      builder.WithId(e => e.Id);
      builder.WithDifferFunc((a, b) => !a.Identical(b));
      builder.OnDifferentRecords((src, tgt) =>
      {
        var sb = new StringBuilder();
        sb.AppendLine($"ID: {src.Id}");
        sb.AppendLine($"+ Old [ Name: {tgt.Name}, SystemName: {tgt.SystemName} ]");
        sb.AppendLine($"- New [ Name: {src.Name}, SystemName: {src.SystemName} ]");
        return sb.ToString();
      });
      builder.OnNewRecords(r =>
      {
        return $"Id: {r.Id,-10} [ Name: {r.Name}, SystemName: {r.SystemName} ]";
      });

      return builder.Build();
    }

    public static IAnalyzeConfig<RoomTemplate> GetRoomTemplate()
    {
      var builder = new AnalyzeConfig<RoomTemplate>.Builder(sCtx => sCtx.RoomTemplate,
        tCtx => tCtx.RoomTemplate);
      builder.WithId(e => e.Id);
      builder.WithDifferFunc((a, b) => !a.Identical(b));
      builder.OnDifferentRecords((src, tgt) =>
      {
        var sb = new StringBuilder();
        sb.AppendLine($"ID: {src.Id}");
        sb.AppendLine($"+ Old [ Name: {tgt.Name}, IsActive: {tgt.IsActive} ]");
        sb.AppendLine($"- New [ Name: {src.Name}, IsActive: {src.IsActive} ]");
        return sb.ToString();
      });
      builder.OnNewRecords(r =>
      {
        return $"Id: {r.Id,-10} [ Name: {r.Name}, IsActive: {r.IsActive} ]";
      });

      return builder.Build();
    }
  }
}
