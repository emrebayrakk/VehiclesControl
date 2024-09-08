using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using VehiclesControl.Domain.Input;
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

        public long AddCar(UserRequest userRequest)
        {
            throw new NotImplementedException();
        }

        public long DeleteCar(long id)
        {
            throw new NotImplementedException();
        }

        public List<CarResponse> GetAll()
        {
            using var connection = GetConnection();
            var carResponse = connection.Query<CarResponse>
                ("SELECT * FROM CARS");
            return carResponse.ToList();
        }

        public CarResponse GetById(long id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var carResponse = connection.QueryFirstOrDefault<CarResponse>
                ("SELECT * FROM CARS WHERE Id = @Id",new {Id =  id});
                if (carResponse != null)
                    return carResponse;
                return null;
            }
        }

        public long UpdateCar(UserRequest userRequest)
        {
            throw new NotImplementedException();
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("GetTheNewsContext"));
        }
    }
}
