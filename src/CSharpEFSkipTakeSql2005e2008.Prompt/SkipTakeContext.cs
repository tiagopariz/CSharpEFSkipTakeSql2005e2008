using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
 
namespace CSharpEFSkipTakeSql2005e2008.Prompt
{ 
    public class SkipTakeContext : DbContext
    {
        private readonly string _connectionString;
 
        public SkipTakeContext()
        {
            var configurationFile = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json")
                .Build();
 
            _connectionString = configurationFile
                                    .GetConnectionString("SkipTakeConnection");
 
        }
 
        public DbSet<Person> People { get; set; }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);            
            base.OnConfiguring(optionsBuilder);
        }
    }
}