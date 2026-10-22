using Moq;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;
using SelfServiceKiosk.Api.Services;

namespace SelfServiceKiosk.Api.Tests.Services;

/// <summary>Unit tests for the category rules. The repository is mocked, so no database is needed.</summary>
public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_categoryRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_CategoriesExist_ReturnsDtosWithIdAndName()
    {
        IReadOnlyList<Category> categories = new List<Category>
        {
            new() { Id = 1, Name = "Drinks" },
            new() { Id = 2, Name = "Snacks" },
        };
        _categoryRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Drinks", result[0].Name);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetAllAsync_NoCategories_ReturnsEmptyList()
    {
        IReadOnlyList<Category> categories = new List<Category>();
        _categoryRepository
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _sut.GetAllAsync();

        Assert.Empty(result);
    }
}
