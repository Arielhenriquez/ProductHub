using System.Linq.Expressions;
using CategoriesModel = ProductHub.Domain.Models.Categories;

namespace ProductHub.Application.Features.Categories.Predicates;

public class CategoriesPredicates
{
    public static Expression<Func<CategoriesModel, bool>> Search(string criteria)
    {
        return (category) =>
            category.Name.Contains(criteria) ||
            category.Description.Contains(criteria) ||
            string.IsNullOrWhiteSpace(criteria);
    }
}
