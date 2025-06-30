using SyncHouseHero.Sync.Impl;

namespace SyncHouseHero
{
  public class Mapper
  {
    public static Dictionary<string, object> EntityAnalyzeConfigMap { get; set; } = new()
        {
          {"HouseType", ImplAnalyzerConfig.GetHouseType() },
          {"PermissionRecord", ImplAnalyzerConfig.GetPermissionRecord() },
        };
  }
}
