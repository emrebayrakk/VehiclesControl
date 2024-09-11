using MongoDB.Driver;

namespace Car.API.Data
{
    public interface IDbConnectionFactory
    {
        IMongoDatabase OpenConnection();
    }
}
