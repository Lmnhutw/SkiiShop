using System.Text.Json;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class SSContextSeed
{
    public static async Task SeedAsync(SS_DbContext context, string contentRootPath)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var candidatePaths = new[]
        {
            Path.Combine(contentRootPath, "Data", "SeedData", "products.json"),
            Path.Combine(contentRootPath, "..", "Infrastructure", "Data", "SeedData", "products.json")
        };
        var seedPath = candidatePaths.FirstOrDefault(File.Exists);

        if (seedPath is null)
        {
            Console.WriteLine("Warning: Seed data file was not found; database was left empty.");
            return;
        }

        try
        {
            var productsData = await File.ReadAllTextAsync(seedPath);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products is null || products.Count == 0)
            {
                Console.WriteLine("Warning: Seed data contained no products.");
                return;
            }

            var now = DateTime.UtcNow;
            foreach (var product in products)
            {
                product.IsVisible = true;
                product.CreatedAt = product.CreatedAt == default ? now : product.CreatedAt;
                product.UpdateAt = product.UpdateAt == default ? now : product.UpdateAt;
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"Successfully seeded {products.Count} products");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error: Invalid JSON in seed data - {ex.Message}");
            throw;
        }
    }
}