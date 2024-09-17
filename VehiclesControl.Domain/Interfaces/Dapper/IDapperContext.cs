using Microsoft.Data.SqlClient;

namespace VehiclesControl.Domain.Interfaces.Dapper
{
    public interface IDapperContext
    {
        SqlConnection CreateConnection();
    }
}
