using IglesiaNet.API.Models.MongoDB;
using MongoDB.Driver;

namespace IglesiaNet.API.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"]
            ?? throw new InvalidOperationException("MongoDB connection string not configured");
        var databaseName = configuration["MongoDB:DatabaseName"]
            ?? "IglesiaNetDb";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<BlogPost> BlogPosts =>
        _database.GetCollection<BlogPost>("BlogPosts");
}
