using Car.API.Data;
using Car.API.Data.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Car.API.Services
{
    public class CarService : ICarService
    {
        private readonly IMongoCollection<Car.API.Data.Entities.Car> carCollection;
        public CarService(IOptions<MongoDbSettings> carDatabaseSetting, IDbConnectionFactory factory)
        {
            var mongoClient = new MongoClient(carDatabaseSetting.Value.Connection);
            var mongoDatabase = mongoClient.GetDatabase(carDatabaseSetting.Value.Database);
            carCollection = factory.OpenConnection().GetCollection<Car.API.Data.Entities.Car>("Cars");
        }

        public async Task AddCarAsync(Data.Entities.Car car)
        {
            await carCollection.InsertOneAsync(car);
        }
    }
}
