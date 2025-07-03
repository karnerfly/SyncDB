using System.Reflection;

namespace SyncHouseHero
{
  public static class ObjectExtension
  {
    public static bool Identical(this object obj1, object obj2)
    {
      if (obj1 == null || obj2 == null)
        return obj1 == obj2;

      Type type1 = obj1.GetType();
      Type type2 = obj2.GetType();

      if (type1 != type2)
        return false;

      foreach (var prop in type1.GetProperties(BindingFlags.Public | BindingFlags.Instance))
      {
        var val1 = prop.GetValue(obj1);
        var val2 = prop.GetValue(obj2);

        if (!Equals(val1, val2))
          return false;
      }

      foreach (var field in type1.GetFields(BindingFlags.Public | BindingFlags.Instance))
      {
        var val1 = field.GetValue(obj1);
        var val2 = field.GetValue(obj2);

        if (!Equals(val1, val2))
          return false;
      }

      return true;
    }
  }
}
