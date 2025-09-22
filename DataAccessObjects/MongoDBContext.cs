using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DataAccessObjects;

public class MongoDBContext
{
    private readonly MongoClient _client;
    public IMongoDatabase Database { get; }

    /// <summary>
    /// Creates a new instance of MongoDBContext, connecting to the database specified in the configuration.
    /// </summary>
    /// <param name="configuration"></param>
    public MongoDBContext(IConfiguration configuration)
    {
        var mongoConn = configuration.GetConnectionString("MongoDB");
        _client = new MongoClient(mongoConn);
        var dbName = configuration.GetSection("MongoDBSettings:DatabaseName").Value;
        Database = _client.GetDatabase(dbName);
    }

    /// <summary>
    /// Creates a new instance of MongoDBContext, connecting to the specified database. Using passed params
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    public MongoDBContext(string connectionString, string databaseName)
    {
        _client = new MongoClient(connectionString);
        Database = _client.GetDatabase(databaseName);
    }
}
