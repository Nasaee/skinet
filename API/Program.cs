using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AddScoped is scope for live time http requet (for each request) until request is finished
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// this is how to inject Generic
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddCors();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Api Docs: baseUrl/scalar/v1
    app.MapScalarApiReference();
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x =>
    x.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:4200", "https://localhost:4200")
);

app.MapControllers();

/*
    เพราะ StoreContext ถูก register เป็น Scoped ตอนนี้เราอยู่นอก HTTP Request → ไม่มี scope ให้อัตโนมัติ ตอนนี้เราอยู่นอก HTTP Request → ไม่มี scope ให้อัตโนมัติ (คิดซะว่า ขอจำลอง request ปลอมขึ้นมาหนึ่งรอบ เพื่อใช้ DbContext)

    using
    ใช้เพื่อ รับประกันว่า resource จะถูก Dispose ทันทีที่ใช้เสร็จ โดยเฉพาะพวก DI Scope / DbContext / DB Connection

    1️⃣ CreateScope() สร้างอะไร?
    ใน scope นี้จะมีของพวก:
    - StoreContext (DbContext)
    - connection ไป DB
    - scoped services ทั้งหมด
    พูดง่าย ๆ นี่คือ “อาณาเขตชั่วคราว” ของ dependency ชุดหนึ่ง

    2️⃣ แล้วทำไมต้อง Dispose?
    พอ scope หมดอายุ:
    - DbContext ต้องถูกปิด
    - DB Connection ต้องคืน pool
    - Memory ต้องถูก release
    ถ้า ไม่ Dispose
    - connection ค้าง
    - memory leak
    - production ช้าลงแบบหาสาเหตุไม่เจอ

    3️⃣ using ทำอะไรให้เรา?
    using var scope = app.Services.CreateScope();
    เทียบเท่า:
    var scope = app.Services.CreateScope();
    try
    {
    // ใช้งาน scope
    }
    finally
    {
    scope.Dispose();
    }

    4️⃣ ทำไมต้องใช้ using ตรงนี้ เป็นพิเศษ?
    - อยู่นอก HTTP request
    - ไม่มี middleware มาจัดการ lifetime ให้
    - เราต้องรับผิดชอบ lifecycle เอง

    ถ้าอยู่ใน Controller: ไม่ต้องใช้ using เพราะ ASP.NET Core จัดการ scope ให้แล้ว
    */

try
{
    using var scope = app.Services.CreateScope();
    // ดึง DbContext จาก DI
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    // create db if not exist yet (Auto DB Upgrade) เทียบเท่า dotnet ef database update
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
