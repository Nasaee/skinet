using System;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreContext context;

    public ProductsController(StoreContext context)
    {
        this.context = context;
    }

    /*
    IEnumerable<T> คือ Interface สำหรับ “ของที่เอาไปวน (foreach) ได้”
    เช่น:
    - List ของ Product
    - Array ของ Product
    - ข้อมูลที่ดึงจาก DB ทีละแถว
    */
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await context.Products.ToListAsync();
    }

    [HttpGet("{id:int}")] // api/products/1
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
            return NotFound();

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        context.Products.Add(product);

        await context.SaveChangesAsync();

        return product;
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !ProductExists(id))
            return BadRequest("Cannot update this product");

        /*
        1. context.Entry(product)
        - บอก EF Core ว่า “ขอเข้าถึง metadata / tracking info ของ object ตัวนี้หน่อย”
        - ปกติ EF จะ track entity ที่ได้มาจาก DB (Find, First, etc.) แต่กรณีนี้ product มาจาก request body → EF ยังไม่รู้จักมัน

        2. .State = EntityState.Modified
        - EF จะ update ทุก column แม้ว่าคุณจะแก้มาแค่ field เดียวก็ตาม
        */
        context.Entry(product).State = EntityState.Modified;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
            return NotFound();

        context.Products.Remove(product);

        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return context.Products.Any(x => x.Id == id);
    }
}
