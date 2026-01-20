using System.Linq.Expressions;

namespace Core.Interfaces;

/*
👉 มันคือ Specification Pattern
👉 เอาไว้ “ห่อเงื่อนไขการ query” ออกจาก Repository
👉 ใช้กับ EF Core / LINQ ได้แบบ type-safe และ compose ได้

Criteria คือ เงื่อนไขหลัก (WHERE clause)

ทำไม interface เขียนแค่ get; ?
เพราะ:
- Interface = สัญญา
- บอกแค่ว่า “ต้องอ่านได้นะ”
- ไม่สนใจว่า implement ยังไง
- get; = อ่านค่าได้อย่างเดียว
*/
public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    bool IsDistinct { get; }
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Select { get; }
}
