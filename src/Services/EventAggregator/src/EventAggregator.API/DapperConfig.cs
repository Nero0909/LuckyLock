using Dapper;
using EventAggregator.Repository.Handlers;

namespace EventAggregator.API
{
    public class DapperConfig
    {
        public static void Init()
        {
            SqlMapper.AddTypeHandler(new DateTimeHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
        }
    }
}
