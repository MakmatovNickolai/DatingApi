using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;

namespace DataAccess.Helpers;

public static class RawSqlExecutor
{
    public static async Task<List<T>> ExecuteAsync<T>(DbContext context, string query, Func<DbDataReader, T> map)
    {
        using (var command = context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            context.Database.OpenConnection();

            using (var result = await command.ExecuteReaderAsync())
            {
                var entities = new List<T>();

                while (result.Read())
                {
                    entities.Add(map(result));
                }

                return entities;
            }
        }
    }
}