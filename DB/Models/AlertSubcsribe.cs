using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment.DB.Models
{
    public class AlertSubscribe : IModelConfiguration
    {
        public int Id { get; set; }
        public required string SubscribeEmail { get; set; }
        public required string Location { get; set; }
        public DateTime SubscribeDate { get; set; }

        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherRecord>(entity =>
            {
            });
        }
    }
}
