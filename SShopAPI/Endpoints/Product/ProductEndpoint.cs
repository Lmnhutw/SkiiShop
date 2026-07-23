using Core.Abstractions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using SShopAPI.DTOs;

namespace SShopAPI.Endpoints;

public static class ProductEndpoint
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (
            int? id,
            string? brand,
            string? type,
            string? sort,
            int? page,
            int? pageSize,
            IRepository<Product> repo) =>
        {
            if (id is not null)
            {
                var product = await repo.GetByIdAsync(id.Value);
                return product is null || !product.IsVisible
                    ? Results.NotFound($"Product {id} not found or not visible.")
                    : Results.Ok(product);
            }

            var currentPage = page.GetValueOrDefault(1);
            var currentPageSize = pageSize.GetValueOrDefault(20);
            if (currentPage < 1 || currentPageSize is < 1 or > 100)
            {
                return Results.BadRequest("page must be at least 1 and pageSize must be between 1 and 100.");
            }

            var query = repo.Query().Where(product => product.IsVisible);

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(product => product.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(product => product.Type == type);
            }

            var orderedQuery = sort?.ToLowerInvariant() switch
            {
                "priceasc" => query.OrderBy(product => product.Price).ThenBy(product => product.Id),
                "pricedesc" => query.OrderByDescending(product => product.Price).ThenBy(product => product.Id),
                "nameasc" => query.OrderBy(product => product.Name).ThenBy(product => product.Id),
                "namedesc" => query.OrderByDescending(product => product.Name).ThenBy(product => product.Id),
                _ => query.OrderBy(product => product.Id)
            };

            var totalCount = await query.CountAsync();
            var items = await orderedQuery
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToListAsync();

            return Results.Ok(new ProductListResponse(
                items,
                totalCount,
                currentPage,
                currentPageSize,
                (int)Math.Ceiling(totalCount / (double)currentPageSize)));
        });

        app.MapGet("/products/brands", async (IRepository<Product> repo) =>
        {
            var brands = await repo.Query()
                .Where(product => product.IsVisible && product.Brand != null && product.Brand != "")
                .Select(product => product.Brand!)
                .Distinct()
                .OrderBy(brand => brand)
                .ToListAsync();

            return Results.Ok(brands);
        });

        app.MapGet("/products/types", async (IRepository<Product> repo) =>
        {
            var types = await repo.Query()
                .Where(product => product.IsVisible && product.Type != null && product.Type != "")
                .Select(product => product.Type!)
                .Distinct()
                .OrderBy(type => type)
                .ToListAsync();

            return Results.Ok(types);
        });

        app.MapPost("/products/create", async (ProductDto productDto, IRepository<Product> repo) =>
        {
            var now = DateTime.UtcNow;
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
                CreatedAt = now,
                UpdateAt = now
            };

            repo.Add(product);
            await repo.SaveChanges();
            return Results.Created($"/products?id={product.Id}", product);
        }).RequireAuthorization();

        app.MapPut("/products/updates/{id:int}", async (int id, ProductDto productDto, IRepository<Product> repo) =>
        {
            var product = await repo.GetByIdAsync(id);
            if (product is null)
            {
                return Results.NotFound();
            }

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.PictureUrl = productDto.PictureUrl;
            product.Brand = productDto.Brand;
            product.Type = productDto.Type;
            product.QuantityInStock = productDto.Quantity;
            product.IsVisible = productDto.IsVisible;
            product.UpdateAt = DateTime.UtcNow;

            repo.Update(product);
            await repo.SaveChanges();
            return Results.Ok(product);
        }).RequireAuthorization();

        app.MapDelete("/products/delete/{id:int}", async (int id, IRepository<Product> repo) =>
        {
            var product = await repo.GetByIdAsync(id);
            if (product is null)
            {
                return Results.NotFound();
            }

            repo.Delete(product);
            await repo.SaveChanges();
            return Results.NoContent();
        }).RequireAuthorization();
    }
}