using Microsoft.Data.Sqlite;

namespace Tags.Repositories.Extensions
{
    public static class SqliteExceptionExtensions
    {
        public static bool IsUniqueConstraintViolation(this SqliteException exception)
        {
            return exception.SqliteErrorCode == 19;
        }
    }
}
