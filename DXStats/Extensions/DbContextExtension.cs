using DXStats.Interfaces;
using DXStats.Services;
using Microsoft.EntityFrameworkCore;

namespace DXStats.Extensions
{
    public static class DbContextExtensions
    {

        public static ISeeder GetSeeder(this DbContext dbContext)
        {
            return ServiceLocator.Current.GetInstance<ISeeder>();
        }
    }
}
