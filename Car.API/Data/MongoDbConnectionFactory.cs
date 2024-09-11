using MongoDB.Driver;

namespace Car.API.Data
{
    public class MongoDbConnectionFactory : IDbConnectionFactory
    {
        private static IMongoDatabase database;
        private IConfiguration Configuration { get; }

        public MongoDbConnectionFactory(IConfiguration configuration)
        {
            Configuration = configuration;

            var config = Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(config.Connection);
            database = client.GetDatabase(config.Database);
        }

        public IMongoDatabase OpenConnection()
        {
            return database;
        }
    }
}
