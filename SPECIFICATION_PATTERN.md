# Specification Pattern - Implementation Summary

## Overview

The Specification Pattern is a design pattern that encapsulates business rules and query logic into reusable, composable objects. In this project, it's used to separate query criteria from the repository layer, making the code more maintainable, testable, and flexible.

## Architecture

### Core Components

#### 1. **ISpecification<T> Interface** (`Core/Interfaces/ISpecification.cs`)
The base interface that defines the contract for specifications:
- `Criteria`: The WHERE clause condition (Expression<Func<T, bool>>)
- `OrderBy`: Ascending ordering expression
- `OrderByDescending`: Descending ordering expression
- `IsDistinct`: Flag for distinct results

#### 2. **ISpecification<T, TResult> Interface**
Extended interface for specifications that transform results:
- Inherits from `ISpecification<T>`
- Adds `Select`: Expression to transform/select specific properties

#### 3. **BaseSpecification<T> Class** (`Core/Specifications/BaseSpecification.cs`)
Base implementation providing:
- Constructor that accepts criteria expression
- Protected constructor for specifications without criteria
- Protected methods: `AddOrderBy()`, `AddOrderByDescending()`, `ApplyDistinct()`
- Properties are read-only from outside, but settable via protected methods

#### 4. **BaseSpecification<T, TResult> Class**
Extended base class for specifications with result transformation:
- Inherits from `BaseSpecification<T>`
- Adds `Select` property and `AddSelect()` method

#### 5. **SpecificationEvaluator<T>** (`Infrastructure/Data/SpecificationEvaluator.cs`)
Static utility class that applies specifications to IQueryable:
- `GetQuery()`: Applies criteria, ordering, and distinct to a query
- `GetQuery<TSpec, TResult>()`: Overload for specifications with result transformation
- Builds the query step by step (WHERE → ORDER BY → DISTINCT → SELECT)

#### 6. **GenericRepository<T>** (`Infrastructure/Data/GenericRepository.cs`)
Repository that uses specifications:
- `ListAsync(ISpecification<T> spec)`: Returns list of entities matching spec
- `GetEntityWithSpec(ISpecification<T> spec)`: Returns single entity
- `ListAsync<TResult>(ISpecification<T, TResult> spec)`: Returns transformed results
- `GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec)`: Returns single transformed result

## Concrete Specifications

### 1. **ProductSpecification** (`Core/Specifications/ProductSpecification.cs`)
Filters products by brand and type, with optional sorting:
```csharp
// Usage: Filters by brand/type (if provided), sorts by price
var spec = new ProductSpecification(brand: "Nike", type: "Shoes", sort: "priceAsc");
var products = await repo.ListAsync(spec);
```

**Features:**
- Optional brand filtering (if null/empty, no filter applied)
- Optional type filtering (if null/empty, no filter applied)
- Sorting: "priceAsc", "priceDesc", or default (priceAsc)

### 2. **BrandListSpecification** (`Core/Specifications/BrandListSpecification.cs`)
Returns distinct brand names:
```csharp
// Usage: Gets all unique brand names
var spec = new BrandListSpecification();
var brands = await repo.ListAsync(spec); // Returns IReadOnlyList<string>
```

**Features:**
- Selects only `Brand` property
- Applies `Distinct()` to remove duplicates

### 3. **TypeListSpecification** (`Core/Specifications/TypeListSpecification.cs`)
Returns distinct type names:
```csharp
// Usage: Gets all unique type names
var spec = new TypeListSpecification();
var types = await repo.ListAsync(spec); // Returns IReadOnlyList<string>
```

**Features:**
- Selects only `Type` property
- Applies `Distinct()` to remove duplicates

## Usage in Controllers

### Example: ProductsController

```csharp
[HttpGet]
public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
    string? brand,
    string? type,
    string? sort
)
{
    var spec = new ProductSpecification(brand, type, sort);
    var products = await repo.ListAsync(spec);
    return Ok(products);
}

[HttpGet("brands")]
public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
{
    var spec = new BrandListSpecification();
    return Ok(await repo.ListAsync(spec));
}
```

## Benefits

### 1. **Separation of Concerns**
- Query logic is separated from repository implementation
- Business rules are encapsulated in specification classes
- Controllers remain clean and focused on HTTP concerns

### 2. **Reusability**
- Specifications can be reused across different parts of the application
- Easy to compose multiple specifications together (future enhancement)

### 3. **Testability**
- Specifications can be tested independently
- Mock specifications for repository testing
- Easy to verify query logic without database

### 4. **Type Safety**
- Compile-time checking of expressions
- IntelliSense support for entity properties
- Prevents runtime errors from typos

### 5. **Maintainability**
- Changes to query logic are centralized in specification classes
- Easy to understand what queries are being executed
- Clear naming conventions (e.g., `ProductSpecification`)

### 6. **Performance**
- Specifications build IQueryable expressions
- EF Core translates to efficient SQL queries
- Lazy evaluation until materialization

## Query Flow

```
Controller
    ↓
Creates Specification (e.g., ProductSpecification)
    ↓
Repository.ListAsync(spec)
    ↓
SpecificationEvaluator.GetQuery(query, spec)
    ↓
Applies: WHERE → ORDER BY → DISTINCT → SELECT
    ↓
Returns IQueryable<T>
    ↓
Materializes to List (via ToListAsync())
    ↓
Returns to Controller
```

## SQL Translation Example

When using `ProductSpecification` with brand="Nike" and type="Shoes":

```csharp
var spec = new ProductSpecification("Nike", "Shoes", "priceAsc");
var products = await repo.ListAsync(spec);
```

**Generated SQL (conceptual):**
```sql
SELECT * 
FROM Products 
WHERE Brand = 'Nike' AND Type = 'Shoes'
ORDER BY Price ASC
```

## Design Decisions

### 1. **Expression Trees vs. Delegates**
- Uses `Expression<Func<T, bool>>` instead of `Func<T, bool>`
- Allows EF Core to translate to SQL
- Enables query optimization by the database

### 2. **Optional Criteria**
- Criteria can be `null` (no filtering)
- Enables "get all" specifications
- Protected constructor in BaseSpecification supports this

### 3. **Protected Methods**
- `AddOrderBy()`, `AddOrderByDescending()`, `AddSelect()` are protected
- Prevents external modification after construction
- Ensures specifications are immutable from outside

### 4. **Generic Constraints**
- `where T : BaseEntity` ensures only entities can be used
- Provides consistent ID property access
- Type safety at compile time

## Future Enhancements

### 1. **Composition**
Add support for combining specifications:
```csharp
var spec = new ProductSpecification(brand, type, sort)
    .And(new PriceRangeSpecification(minPrice, maxPrice));
```

### 2. **Pagination**
Add `Skip` and `Take` properties:
```csharp
public int? Skip { get; private set; }
public int? Take { get; private set; }
```

### 3. **Includes (Eager Loading)**
Add support for related entities:
```csharp
public List<Expression<Func<T, object>>> Includes { get; } = new();
```

### 4. **Count Specification**
Add method to get count without materializing:
```csharp
Task<int> CountAsync(ISpecification<T> spec);
```

## Conclusion

The Specification Pattern in this project provides a clean, maintainable way to handle complex queries while keeping the repository layer generic and reusable. It successfully separates business logic from data access, making the codebase more testable and easier to understand. The implementation is well-structured with clear separation between interfaces, base classes, concrete specifications, and the evaluator that applies them.

**Key Takeaways:**
- ✅ Query logic is encapsulated in specification classes
- ✅ Repository remains generic and reusable
- ✅ Type-safe expressions enable compile-time checking
- ✅ EF Core efficiently translates to SQL
- ✅ Easy to extend with new specifications
- ✅ Clean separation of concerns throughout the stack
