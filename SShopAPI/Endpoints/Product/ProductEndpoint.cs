namespace SShopAPI.Endpoints.Product
{
    public static class ProductEndpoint
    {
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (IProductService ProductService) =>
            {
                var products = await productService.GetAllProductsAsync();
                return Results.Ok(products);
            });
            app.MapGet("/products/{id}", async (int id, IProductService productService) =>
            {
                var product = await productService.GetProductByIdAsync(id);
                return product is not null ? Results.Ok(product) : Results.NotFound();
            });
            app.MapPost("/products", async (ProductDto productDto, IProductService productService) =>
            {
                var createdProduct = await productService.CreateProductAsync(productDto);
                return Results.Created($"/products/{createdProduct.Id}", createdProduct);
            });
            app.MapPut("/products/{id}", async (int id, ProductDto productDto, IProductService productService) =>
            {
                var updatedProduct = await productService.UpdateProductAsync(id, productDto);
                return updatedProduct is not null ? Results.Ok(updatedProduct) : Results.NotFound();
            });
            app.MapDelete("/products/{id}", async (int id, IProductService productService) =>
            {
                var deleted = await productService.DeleteProductAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}