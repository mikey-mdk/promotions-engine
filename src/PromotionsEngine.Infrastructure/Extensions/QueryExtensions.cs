using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace PromotionsEngine.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class QueryExtensions
{
    public static IQueryable<T> AddFilterIfValueExists<T>(this IQueryable<T> query, int property, Expression<Func<T, bool>> predicate)
    {
        return property > 0 ? query.Where(predicate) : query;
    }

    public static IQueryable<T> AddFilterIfValueExists<T>(this IQueryable<T> query, string property, Expression<Func<T, bool>> predicate)
    {
        return !string.IsNullOrWhiteSpace(property) ? query.Where(predicate) : query;
    }

    public static IQueryable<T> AddFilterIfValueExists<T>(this IQueryable<T> query, DateTime? property,
        Expression<Func<T, bool>> predicate)
    {
        return property.HasValue ? query.Where(predicate) : query;
    }
}