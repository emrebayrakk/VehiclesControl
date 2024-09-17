using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using VehiclesControl.Domain.Interfaces.Dapper;

namespace VehiclesControl.Data.Repositories.Dapper.DapperORM
{
    public class DapperContext : IDapperContext
    {
        public SqlConnection CreateConnection()
        {
            return new SqlConnection("Data Source=DESKTOP-P87BUPQ;Initial Catalog=VehiclesControl;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}
