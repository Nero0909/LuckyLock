using Dapper;
using Tags.Repositories;
using Tags.Repositories.Handlers;

namespace Tags.API
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
