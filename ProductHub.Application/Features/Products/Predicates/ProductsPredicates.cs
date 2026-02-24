using System.Linq.Expressions;
using ProductsModel = ProductHub.Domain.Models.Products;

namespace ProductHub.Application.Features.Products.Predicates;

public static class ProductsPredicates
{
    public static Expression<Func<ProductsModel, bool>> Search(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return _ => true;
        }

        search = search.Trim();
        return product => product.Name.Contains(search) || (product.Description != null && product.Description.Contains(search));
    }
}
