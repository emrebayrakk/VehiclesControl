using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using VehiclesControl.Domain.Interfaces.Dapper;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.Repositories.Dapper
{
    public class CarRepositoryDapper : ICarRepositoryDapper
    {
        private readonly IConfiguration _configuration;

        public CarRepositoryDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<CarResponse> GetAll()
        {
            using var connection = GetConnection();
            var carResponse = connection.Query<CarResponse>
                ("SELECT * FROM CARS");
            return carResponse.ToList();
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("GetTheNewsContext"));
        }
    }
}
