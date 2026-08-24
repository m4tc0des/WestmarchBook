using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Infrastructure.DataAccess;

namespace WestmarchBook.Infrastructure.Migrations;

public class DatabaseMigration
{
    public static async Task ExecuteMigrations(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<WestmarchBookDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
