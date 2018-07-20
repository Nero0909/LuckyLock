using System;
using System.Data;
using System.Globalization;
using Dapper;

namespace Tags.Repositories.Handlers
{
    public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value;
        }

        public override DateTime Parse(object value)
        {
            var date = DateTime.Parse(value.ToString());
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }
    }
}
