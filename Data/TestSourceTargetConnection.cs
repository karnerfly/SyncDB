namespace SyncHouseHero.Data
{
  internal class TestSourceTargetConnection
  {
    public static void Test(SourceContext src, TargetContext tgt)
    {

      if (!src.Database.CanConnect())
      {
        throw new Exception("can not connect with source database");
      }

      if (!tgt.Database.CanConnect())
      {
        throw new Exception("can not connect with target database");
      }
    }

  }
}
