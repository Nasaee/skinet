using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    /*
    แปลว่า:
    - ถ้า ไม่ส่ง brand/type → true → ไม่ filter
    - ถ้าส่ง → ต้อง p.Brand == brand
    - ถ้าส่ง → ต้อง p.Type == type

    รวมด้วย &&
    brand condition
    AND
    type condition

    EF Core จะทำประมาณนี้ (conceptual): (ประเมินทีละ record)
    ----------------------------------------
    foreach (var p in Products)
    {
        bool result =
            (false || p.Brand == "Nike")
        && (false || p.Type == "Shoes");

        if (result)
            yield return p;
    }

    ----------------------------------------
    */
    public ProductSpecification(string? brand, string? type)
        : base(p =>
            (string.IsNullOrWhiteSpace(brand) || p.Brand == brand)
            && (string.IsNullOrWhiteSpace(type) || p.Type == type)
        ) { }
}
