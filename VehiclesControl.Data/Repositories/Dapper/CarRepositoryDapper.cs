using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using VehiclesControl.Domain.Entities;
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

        public long AddCar(CarRequest carRequest)
        {
            Car newcar = new Car()
            {
                Color = carRequest.Color,
                HeadlightsOn = carRequest.HeadlightsOn,
                Wheels = carRequest.Wheels,
                CreatedDate = DateTime.Now
            };
            
            using var connection = GetConnection();
            var res = connection.Execute("INSERT INTO CARS (Color,HeadlightsOn,Wheels,CreatedDate) VALUES (@Color, @HeadlightsOn, @Wheels, @CreatedDate)", newcar);
            return res;
        }

        public long DeleteCar(long id)
        {
            using var connection = GetConnection();
            var carResponse = connection.Execute
                ("DELETE FROM CARS WHERE Id = @Id", new { Id = id });
            return (long)carResponse;

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

        public long UpdateCar(CarRequest carRequest)
        {
            

            using var connection = GetConnection();
            var carResponse = connection.QueryFirstOrDefault<Car>
                ("SELECT * FROM CARS WHERE Id = @Id", new { Id = carRequest.Id });
            Car newcar = new Car()
            {
                Id = carRequest.Id.GetValueOrDefault(),
                Color = carRequest.Color,
                HeadlightsOn = carRequest.HeadlightsOn,
                Wheels = carRequest.Wheels,
                UpdatedDate = DateTime.Now,
                CreatedDate = carResponse != null ? carResponse.CreatedDate : DateTime.Now,
            };
            var res = connection.Execute("UPDATE CARS SET Color = @Color, HeadlightsOn = @HeadlightsOn, Wheels = @Wheels, UpdatedDate = @UpdatedDate, CreatedDate = @CreatedDate WHERE Id = @Id", newcar);
            return res;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
