using Microsoft.EntityFrameworkCore;

namespace ANG_Assessment.DB.Models
{
    public interface IModelConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
