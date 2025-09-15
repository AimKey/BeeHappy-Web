using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace DataAccessObjects
{
    public class BeeHappyContext : DbContext
    {
        public BeeHappyContext(DbContextOptions options) : base(options)
        {
        }

        public BeeHappyContext()
        {
        }

        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies() // Allow lazy loading like spring boot 
                .UseSqlServer(GetConnectionString());
        }

        // Configure the relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeeHappyContext).Assembly);
            // modelBuilder.ApplyConfiguration(new UserDetailConfiguration());

            modelBuilder.Entity<TestObject>().HasData(
                [
                    new TestObject { Id = ObjectId.GenerateNewId().ToString(), UserName = "User 1" },
                    new TestObject { Id = ObjectId.GenerateNewId().ToString(), UserName = "User 2" },
                ]
            );
        }

        // DbSet
        public DbSet<TestObject> TestObjects { get; set; }
    }
}