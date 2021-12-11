using Microsoft.EntityFrameworkCore;

namespace hexa_droid.Data.Seedwork
{
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
                        //Attributes = new Dictionary<string, string> { { "age", "29.5" }, { "eyes", "yes" } },
                    },
                    new User
                    {
                        Id = 2,
                        Name = "Toni",
                        Email = "tonii@email.com",
                        //Attributes = new Dictionary<string, string> { { "age", "29" }, { "eyes", "yes" } },
                    });

                context.SaveChanges();
            }
        }
    }
}
