using Core.Entities;

namespace Core.Interfaces;

//  where T : BaseEntity คือ “T ต้องเป็นคลาสที่สืบทอดจาก BaseEntity เท่านั้น”
// BaseEntity = คลาสแม่ (base class) ที่เอาไว้เก็บ คุณสมบัติร่วม (common properties) ของ Entity ทุกตัวในระบบ
public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T?> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveAllAsync();
    bool Exists(int id);
}
