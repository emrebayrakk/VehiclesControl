using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Car.API.Data.Entities
{
    public abstract class Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
        DateTime CreatedDate { get; set; }
    }
}
