using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data
{
    public class SSContextSeed
    {
        public static async Task SeedAsync(SS_DbContext context)
        {
            // Seed initial data if necessary
            if (!context.Products.Any())
            {
                try
                {
                    var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    if (products != null && products.Count > 0)
                    {
                        context.AddRange(products);
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Successfully seeded {products.Count} products");
                    }
                    else
                    {
                        Console.WriteLine("Warning: No products found in seed data or deserialization returned null");
                        // Optionally throw an exception if seeding is critical
                        // throw new InvalidOperationException("Failed to load seed data");
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Warning: Seed data file not found at ../Infrastructure/Data/SeedData/products.json");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error: Invalid JSON in seed data - {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during seeding: {ex.Message}");
                    throw; // Re-throw if seeding is critical for your app
                }
            }
        }
    }
}