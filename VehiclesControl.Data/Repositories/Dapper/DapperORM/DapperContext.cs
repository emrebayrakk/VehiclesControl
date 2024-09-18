using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using VehiclesControl.Domain.Interfaces.Dapper;

namespace VehiclesControl.Data.Repositories.Dapper.DapperORM
{
    public class DapperContext : IDapperContext
    {
        private readonly ContextOption _contextOption;

        public DapperContext(IOptions<ContextOption> contextOption)
        {
            _contextOption = contextOption.Value;
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_contextOption.DefaultConnection);
        }
    }
}
