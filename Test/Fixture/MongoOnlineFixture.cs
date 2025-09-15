using DataAccessObjects;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NUnit.Framework;
using Repositories.Generics;
using Test.BusinessObjects;

namespace Test.Fixture
{
    public class MongoOnlineFixture
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public IMongoClient Client { get; }

        public MongoOnlineFixture()
        {
            var fileConfig = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json", optional: true)
                .Build();

            var fileConn = fileConfig.GetConnectionString("MongoDB");

            // 2) Prefer NUnit param, then env var, then file
            var fromParam = TestContext.Parameters["mongodbUri"];
            var fromEnv = Environment.GetEnvironmentVariable("MONGODB_URI");

            ConnectionString =
                !string.IsNullOrWhiteSpace(fromParam) ? fromParam :
                !string.IsNullOrWhiteSpace(fromEnv) ? fromEnv :
                !string.IsNullOrWhiteSpace(fileConn) ? fileConn :
                throw new InvalidOperationException(
                    "MongoDB URI not provided. Use NUnit param mongodbUri, env var MONGODB_URI, or testsettings.json.");

            // Unique DB per run/class
            DatabaseName = $"repo_{Guid.NewGuid():N}".Substring(0, 20).ToLowerInvariant();

            Client = new MongoClient(ConnectionString);
        }

        public MongoDBContext CreateContext() => new MongoDBContext(ConnectionString, DatabaseName);

        public GenericRepository<Dummy> CreateRepo() => new GenericRepository<Dummy>(CreateContext());

        public void DropDatabase() => Client.DropDatabase(DatabaseName);
    }
}
