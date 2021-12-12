using Microsoft.EntityFrameworkCore;

namespace hexa_droid.Data.Seedwork;

public class DataGenerator
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApiContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApiContext>>()))
        {
            if (context.Users.Any())
            {
                return;
            }

            context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Name = "Josh",
                    Email = "joshh@email.com",
                    Attributes = new UserAttributes()
                    {
                        Age = 29,
                        IsEnabled = true,
                    }
                },
                new User
                {
                    Id = 2,
                    Name = "Toni",
                    Email = "tonii@email.com",
                    Attributes = new UserAttributes()
                    {
                        Age = 29,
                        IsEnabled = true,
                    }
                });

            context.SaveChanges();
        }
    }
}
