using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    // static means: we can use this method without needing to create new instance of StoreContextSeed
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {
            var productData = await File.ReadAllTextAsync(
                "../Infrastructure/Data/SeedData/products.json"
            );

            var products = JsonSerializer.Deserialize<List<Product>>(productData);

            if (products is null)
                return;

            context.Products.AddRange(products);

            await context.SaveChangesAsync();
        }
    }
}
