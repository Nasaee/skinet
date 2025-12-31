using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

/*
ปัญหาถ้า “ไม่ config Price”
ถ้าเขียนแบบนี้เฉย ๆ
----------------------------------
public decimal Price { get; set; }
----------------------------------
EF Core จะทำแบบนี้เบื้องหลัง:
decimal(18,2)   ❌ ไม่เสมอไป
decimal(18,0)   ❌ บ่อยมาก

ผลคือ:
Price = 199.99
↓
DB เก็บ = 200   😱

หรือบางกรณี:
- truncate ทศนิยม
- rounding แบบไม่บอก
- warning ตอน migration

ทำไมต้อง HasColumnType("decimal(18,2)") ?
--------------------------------------
builder.Property(x => x.Price)
       .HasColumnType("decimal(18,2)");
--------------------------------------
18 -> 18 หลัก
2 ->ทศนิยม 2 ตำแหน่ง
ex: 999,999,999,999,999.99
*/

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Name).IsRequired(); // optional you can add more
    }
}
