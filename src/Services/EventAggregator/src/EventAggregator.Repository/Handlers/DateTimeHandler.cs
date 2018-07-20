using System;
using System.Data;
using Dapper;

namespace EventAggregator.Repository.Handlers
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
