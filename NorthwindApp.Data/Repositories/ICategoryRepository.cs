using NorthwindApp.Entities.Models;

namespace NorthwindApp.Data.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Şimdilik özel bir method yok, temel IRepository yeterli
    }
}
