using Core.Abstractions;
using Core.Entities;
using SShopAPI.DTOs;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SShopAPI.Endpoints
{
    public static class ProductEndpoint
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (int? id, string? brand, string? type, string sort, IRepository<Product> repo) =>
            {
                // If ID is provided → return single product
                if (id is not null)
                {
                    var product = await repo.GetByIdAsync(id.Value);
                    if (product is null || !product.IsVisible)
                        return Results.NotFound($"Product {id} not found or not visible.");

                    return Results.Ok(product);
                }

                // No ID → get all products
                var products = await repo.GetAllAsync();

                // Base query (only visible)
                var query = products.Where(p => p.IsVisible);

                if (!string.IsNullOrWhiteSpace(brand))
                {
                    query = query.Where(p => p.Brand != null &&
                                             p.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(type))
                {
                    query = query.Where(p => p.Type != null &&
                                             p.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(sort))
                {
                    query = sort.ToLower() switch
                    {
                        "priceasc" => query.OrderBy(p => p.Price),
                        "pricedesc" => query.OrderByDescending(p => p.Price),
                        "nameasc" => query.OrderBy(p => p.Name),
                        "namedesc" => query.OrderByDescending(p => p.Name),
                        _ => query
                    };
                }

                return Results.Ok(query);
            });


            // brands list
            app.MapGet("/products/brands", async (IRepository<Product> repo) =>
            {
                var products = await repo.GetAllAsync();

                var brands = products
                    .Where(p => p.IsVisible && !string.IsNullOrWhiteSpace(p.Brand))
                    .Select(p => p.Brand!)
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                return Results.Ok(brands);
            });

            // type list
            app.MapGet("/products/types", async (IRepository<Product> repo) =>
            {
                var products = await repo.GetAllAsync();

                var types = products
                    .Where(p => p.IsVisible && !string.IsNullOrWhiteSpace(p.Type))
                    .Select(p => p. Type!)
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                return Results.Ok(types);
            });

            app.MapPost("/products/create", async (ProductDto productDto, IRepository<Product> repo) =>
            {
                // Map DTO to entity
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    PictureUrl = productDto.PictureUrl,
                    Brand = productDto.Brand,
                    Type = productDto.Type,
                    QuantityInStock = productDto.Quantity,
                    IsVisible = productDto.IsVisible,

                    // BaseEntity props
                    CreatedAt = DateTime.Now,
                    //CreatedBy = UserLoginInfo,
                    UpdateAt = DateTime.Now,
                    // UpdatedBy = username
                };
                repo.Add(product);
                await repo.SaveChanges();
                return Results.Created($"/products/create/{product.Id}", product);
            });

            app.MapPut("/products/updates/{id:int}", async (int id, ProductDto productDto, IRepository<Product> repo) =>

            {
                var existingProduct = await repo.GetByIdAsync(id);
                if (existingProduct is null) return Results.NotFound();
                var updateProduct = existingProduct;

                // Update properties from DTO
                updateProduct.Name = productDto.Name;
                updateProduct.Price = productDto.Price;
                updateProduct.Description = productDto.Description;
                updateProduct.PictureUrl = productDto.PictureUrl;
                updateProduct.Brand = productDto.Brand;
                updateProduct.Type = productDto.Type;
                updateProduct.QuantityInStock = productDto.Quantity;
                updateProduct.IsVisible = productDto.IsVisible;

                repo.Add(updateProduct);
                return Results.Ok(updateProduct);
            });

            app.MapDelete("/products/delete/{id:int}", async (int id, IRepository<Product> repo) =>
            {
                var existing = await repo.GetByIdAsync(id);
                if (existing is null) return Results.NotFound();

                // var hasOrders = false; // TODO: Check if product is associated with any orders
                var deletedProduct = existing;

                repo.Delete(deletedProduct);
                await repo.SaveChanges();
                return Results.NoContent();
            });
        }
    }
}