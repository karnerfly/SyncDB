using SyncHouseHero.Sync.Impl;

namespace SyncHouseHero
{
  public class Mapper
  {
    public static Dictionary<string, object> EntityAnalyzeConfigMap { get; set; } = new()
        {
          {"HouseType", ImplAnalyzeConfig.GetHouseType() },
          {"PermissionRecord", ImplAnalyzeConfig.GetPermissionRecord() },
          {"RoomTemplate", ImplAnalyzeConfig.GetRoomTemplate() },
        };
  }
}
