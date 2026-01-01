using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context) : IProductRepository
{
    public void AddProduct(Product product)
    {
        context.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {
        context.Products.Remove(product);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await context.Products.Select(p => p.Brand).Distinct().ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(
        string? brand,
        string? type,
        string? sort
    )
    {
        var query = context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(p => p.Brand == brand);
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.Type == type);

        // switch expression (ของใหม่ เขียนสั้นกว่า 🔥)
        query = sort switch
        {
            "priceAsc" => query.OrderBy(p => p.Price),
            "priceDesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name), // Default
        };

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await context.Products.Select(p => p.Type).Distinct().ToListAsync();
    }

    public bool ProductExists(int id)
    {
        return context.Products.Any(p => p.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void UpdateProduct(Product product)
    {
        /*
        1. context.Entry(product)
        - บอก EF Core ว่า “ขอเข้าถึง metadata / tracking info ของ object ตัวนี้หน่อย”
        - ปกติ EF จะ track entity ที่ได้มาจาก DB (Find, First, etc.) แต่กรณีนี้ product มาจาก request body → EF ยังไม่รู้จักมัน

        2. .State = EntityState.Modified
        - EF จะ update ทุก column แม้ว่าคุณจะแก้มาแค่ field เดียวก็ตาม
        */
        context.Entry(product).State = EntityState.Modified;
    }
}
