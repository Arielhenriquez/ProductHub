using System.Linq.Expressions;
using ProductHub.Application.Features.Categories.Dtos;
using ProductHub.Application.Features.Products.Dtos;
using ProductsModel = ProductHub.Domain.Models.Products;

namespace ProductHub.Application.Features.Products.Projections;

public static class ProductsProjections
{
    public static Expression<Func<ProductsModel, ProductResponseDto>> Search => product => new ProductResponseDto
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        IsActive = product.IsActive,
        QuantityInStock = product.QuantityInStock,
        CategoryResponses = product.Category == null
           ? new List<CategoryResponseDto>()
           : new List<CategoryResponseDto>
           {
                new CategoryResponseDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                }
           },
        Images = product.Images.Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.IsMain)
            .ThenBy(i => i.SortOrder)
            .Select(i => new ProductImageResponseDto
            {
                Id = i.Id,
                Url = i.Url,
                IsMain = i.IsMain,
                SortOrder = i.SortOrder
            }).ToList()
    };
}
