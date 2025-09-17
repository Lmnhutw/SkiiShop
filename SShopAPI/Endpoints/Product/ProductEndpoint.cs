using Core.Abstractions;
using Core.Entities;
using SShopAPI.DTOs;

namespace SShopAPI.Endpoints
{
    public static class ProductEndpoint
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (int? id, IRepository<Product> repo) =>
            {
                if (id is null)
                {
                    var products = await repo.GetAllAsync();

                    var visibleProducts = products
                        .Where(p => p.IsVisible); // visible flag

                    return Results.Ok(visibleProducts);
                }

                // find by id
                var product = await repo.GetByIdAsync(id.Value);
                if (product is null)
                {
                    return Results.NotFound();
                }

                if (!product.IsVisible)
                {
                    return Results.Ok(); // visible flag
                }

                return Results.Ok(product);
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
                    Quantity = productDto.Quantity,
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
                updateProduct.Quantity = productDto.Quantity;
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