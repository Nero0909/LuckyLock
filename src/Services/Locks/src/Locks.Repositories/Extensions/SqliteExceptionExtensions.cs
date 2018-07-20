using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Locks.Repository.Extensions
{
    public static class SqliteExceptionExtensions
    {
        public static bool IsUniqueConstraintViolation(this SqliteException exception)
        {
            return exception.SqliteErrorCode == 19;
        }
    }
}
