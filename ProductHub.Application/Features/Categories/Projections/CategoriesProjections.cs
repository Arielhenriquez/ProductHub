using System.Linq.Expressions;
using ProductHub.Application.Features.Categories.Dtos;
using CategoriesModel = ProductHub.Domain.Models.Categories;


namespace ProductHub.Application.Features.Categories.Projections;

public class CategoriesProjections
{
    public static Expression<Func<CategoriesModel, CategoryResponseDto>> Search =>
        (category) => new CategoryResponseDto()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
}
