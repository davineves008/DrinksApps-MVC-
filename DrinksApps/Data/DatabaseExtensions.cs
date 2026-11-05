using Microsoft.EntityFrameworkCore;

namespace DrinksApps.Data
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "ConnectionString 'DefaultConnection' não configurada. " +
                    "Verifique o secrets.json ou a variável " +
                    "ConnectionStrings__DefaultConnection.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}