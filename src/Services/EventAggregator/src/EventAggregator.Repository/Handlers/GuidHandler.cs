using System;
using Dapper;

namespace EventAggregator.Repository.Handlers
{
    public class GuidHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            var inVal = (byte[]) value;
            return new Guid(inVal);
        }

        public override void SetValue(System.Data.IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToByteArray();
        }
    }
}
