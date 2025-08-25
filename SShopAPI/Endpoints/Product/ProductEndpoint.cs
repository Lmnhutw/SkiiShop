using Core.Abstractions;
using Core.Entities;
using SShopAPI.DTOs;
using Microsoft.AspNetCore.Builder;

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
                var productDtos = products
                    .Where(p => p.IsVisible) // visible flag
                    .Select(p => new ProductDto
                    {
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,
                        PictureUrl = p.PictureUrl,
                        Brand = p.Brand,
                        Quantity = p.Quantity,
                        IsVisible = p.IsVisible,
                        CreatedAt = p.CreatedAt
                    });

                return Results.Ok(productDtos);
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

                var dto = new ProductDto
                {
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    PictureUrl = product.PictureUrl,
                    Brand = product.Brand,
                    Quantity = product.Quantity,
                    IsVisible = product.IsVisible,
                    CreatedAt = product.CreatedAt
                };

                return Results.Ok(dto);
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
                    CreatedAt = productDto.CreatedAt
                };
                await repo.AddAsync(product);
                return Results.Created($"/products/create/{product.Id}", product);
            });

            app.MapPut("/products/updates/{id:int}", async (int id, ProductDto productDto, IRepository<Product> repo) =>
        
            {
               
                var existing = await repo.GetByIdAsync(id);
                if (existing is null) return Results.NotFound();
    
                // Update properties from DTO
                existing.Name = productDto.Name;
                existing.Price = productDto.Price;
                existing.Description = productDto.Description;
                existing.PictureUrl = productDto.PictureUrl;
                existing.Brand = productDto.Brand;
                existing.Quantity = productDto.Quantity;
                existing.IsVisible = productDto.IsVisible;
            
                await repo.UpdateAsync(existing);
                return Results.Ok(existing);
            });

            app.MapDelete("/products/delete/{id:int}", async (int id, IRepository<Product> repo) =>
            {
                var existing = await repo.GetByIdAsync(id);
                if (existing is null) return Results.NotFound();

                await repo.DeleteAsync(id);
                return Results.NoContent();
            });
        }
    }
}