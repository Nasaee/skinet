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
    public ProductSpecification(ProductSpecParams specParams)
        : base(p =>
            (specParams.Brands.Count == 0 || specParams.Brands.Contains(p.Brand))
            && (specParams.Types.Count == 0 || specParams.Types.Contains(p.Type))
        )
    {
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        switch (specParams.Sort)
        {
            case "priceAsc":
                AddOrderBy(p => p.Price);
                break;
            case "priceDesc":
                AddOrderByDescending(p => p.Price);
                break;
            default:
                AddOrderBy(p => p.Price);
                break;
        }
    }
}
