using Microsoft.VisualBasic;

namespace Infrastructure.Data
{
    public class SSContextSeed
    {
        public static async Task SeedAsync(SS_DbContext context)
        {
            // Seed initial data if necessary
            if (!context.Products.Any())
            {
                context.Products.AddRange(new[]
                {
                    VariantType productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            });
            await context.SaveChangesAsync();
        }
    }
}
}