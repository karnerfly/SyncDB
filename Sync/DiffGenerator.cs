using Microsoft.EntityFrameworkCore;
using SyncHouseHero.Data;
using SyncHouseHero.Models;
using System.Data.Common;

namespace SyncHouseHero.Sync
{
  public class DiffGenerator(string sourceString, string targetString)
  {
    private readonly string _sourceString = sourceString;
    private readonly string _targetString = targetString;

    private static DbCommand CreatCommand(DbConnection conn, string tableName, string idColumn)
    {
      var sql = $@"SELECT ""{idColumn}"" as Id, md5(row_to_json(t)::text) AS Hash FROM ""{tableName}"" t";
      var command = conn.CreateCommand();
      command.CommandText = sql;
      return command;
    }

    private static async Task<List<HashResult>> PrepareHashResultsFromReader(DbDataReader reader)
    {
      var results = new List<HashResult>();
      while (await reader.ReadAsync())
      {
        results.Add(new HashResult
        {
          Id = reader.GetInt32(0),
          Hash = reader.GetString(1)
        });
      }
      return results;
    }

    private async Task<List<HashResult>> GetSourceHashResultsAsync(string tableName, string idColumn = "Id")
    {
      var context = new SourceContext(_sourceString);
      var conn = context.Database.GetDbConnection();

      await conn.OpenAsync();

      var reader = await CreatCommand(conn, tableName, idColumn).ExecuteReaderAsync();
      return await PrepareHashResultsFromReader(reader);
    }

    private async Task<List<HashResult>> GetTargetHashResultsAsync(string tableName, string idColumn = "Id")
    {
      var context = new TargetContext(_targetString);
      var conn = context.Database.GetDbConnection();

      await conn.OpenAsync();

      await using var reader = await CreatCommand(conn, tableName, idColumn).ExecuteReaderAsync();
      return await PrepareHashResultsFromReader(reader);
    }

    public async Task Analyze(string table, string idColumn = "Id")
    {
      var sourceTask = GetSourceHashResultsAsync(table, idColumn);
      var targetTask = GetTargetHashResultsAsync(table, idColumn);

      await Task.WhenAll(sourceTask, targetTask);

      var sourceResult = sourceTask.Result;
      var targetResult = targetTask.Result;

      PrintSyncSummary(table, sourceResult, targetResult);
    }

    private static void PrintSyncSummary(string tableName, List<HashResult> sourceResult, List<HashResult> targetResult)
    {
      int padding = 60;
      var sourceMap = sourceResult.ToDictionary(x => x.Id, x => x.Hash);
      var targetMap = targetResult.ToDictionary(x => x.Id, x => x.Hash);

      var toInsert = sourceMap.Keys.Except(targetMap.Keys).ToList();
      var toDelete = targetMap.Keys.Except(sourceMap.Keys).ToList();
      var toUpdate = sourceMap
          .Where(kv => targetMap.ContainsKey(kv.Key) && targetMap[kv.Key] != kv.Value)
          .Select(kv => kv.Key)
          .ToList();

      var same = sourceMap.Keys
          .Where(id => targetMap.ContainsKey(id) && sourceMap[id] == targetMap[id])
          .ToList();

      Console.WriteLine("=".PadRight(padding, '='));
      Console.WriteLine($"Table: {tableName}");
      Console.WriteLine("-".PadRight(padding, '-'));

      Console.WriteLine($"{"Source Records",-20}: {sourceResult.Count}");
      Console.WriteLine($"{"Target Records",-20}: {targetResult.Count}");
      Console.WriteLine($"{"Identical Records",-20}: {same.Count}");
      Console.WriteLine($"{"To Insert",-20}: {toInsert.Count}");
      Console.WriteLine($"{"To Delete",-20}: {toDelete.Count}");
      Console.WriteLine($"{"To Update",-20}: {toUpdate.Count}");

      Console.WriteLine("-".PadRight(padding, '-'));

      PrintIdList("Insert IDs", toInsert);
      PrintIdList("Delete IDs", toDelete);
      PrintIdList("Update IDs", toUpdate);

      Console.WriteLine("=".PadRight(padding, '='));
      Console.WriteLine();
    }

    private static void PrintIdList(string label, List<int> ids)
    {
      Console.WriteLine($"{label,-20}: {(ids.Count == 0 ? "None" : string.Join(", ", ids))}");
    }
  }
}
