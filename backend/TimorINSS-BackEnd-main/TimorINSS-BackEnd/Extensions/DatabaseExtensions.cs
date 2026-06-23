namespace TimorINSSBackEnd.Extensions
{
    public static class DatabaseExtensions
    {
        //public static List<T> SqlQuery<T>(string query, Func<DbDataReader, T> map)
        //{
        //    using (var context = new DbContext())
        //    {
        //        using (var command = context.Database.GetDbConnection().CreateCommand())
        //        {
        //            command.CommandText = query;
        //            command.CommandType = CommandType.Text;

        //            context.Database.OpenConnection();

        //            using (var result = command.ExecuteReader())
        //            {
        //                var entities = new List<T>();

        //                while (result.Read())
        //                {
        //                    entities.Add(map(result));
        //                }

        //                return entities;
        //            }
        //        }
        //    }
        //}
    }
}