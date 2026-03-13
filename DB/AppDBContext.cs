using ANG_Assessment.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ANG_Assessment.DB
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<WeatherRecord> WeatherRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IModelConfiguration).IsAssignableFrom(t)
                            && !t.IsInterface
                            && !t.IsAbstract);

            foreach (var type in types)
            {
                var instance = (IModelConfiguration)Activator.CreateInstance(type)!;
                instance.Configure(modelBuilder);
            }
        }
    }
}
