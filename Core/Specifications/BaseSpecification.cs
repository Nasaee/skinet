using System;
using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{
    /*
    protected constructor (ไม่มีเงื่อนไข)
    ความหมาย:
    - ให้ subclass เรียกได้
    - สร้าง Spec แบบ ไม่มี Criteria ได้
    เช่น:
    -------------------------------------------------------------
    public class AllProductsSpec : BaseSpecification<Product>
    {
        public AllProductsSpec() : base() { }
    }
    --------------------------------------------------------------
    ผลลัพธ์: Criteria == null
    */
    protected BaseSpecification()
        : this(null) { }

    public Expression<Func<T, bool>>? Criteria => criteria;
}
