using Dapper;
using Locks.Repository.Handlers;

namespace Locks.API
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
