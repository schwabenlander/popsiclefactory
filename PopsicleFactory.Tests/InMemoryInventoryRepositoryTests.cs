using PopsicleFactory.Api.Models;
using PopsicleFactory.Api.Repositories;

namespace PopsicleFactory.Tests;

public class InMemoryInventoryRepositoryTests
{
    [Fact]
    public async Task GetAllPopsicles_ReturnsAllPopsicles()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.GetAllPopsicles();

        // Assert
        var resultsList = result.ToList(); // Convert to list to prevent multiple enumeration
        Assert.Equal(3, resultsList.Count);
        Assert.Contains(resultsList, p => p.Name == "Strawberry Delight");
        Assert.Contains(resultsList, p => p.Name == "Blueberry Bliss");
        Assert.Contains(resultsList, p => p.Name == "Orange Creamsicle");
    }

    [Fact]
    public async Task GetPopsicleByIdAsync_WithExistingId_ReturnsPopsicle()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.GetPopsicleByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Strawberry Delight", result.Name);
    }

    [Fact]
    public async Task GetPopsicleByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.GetPopsicleByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePopsicleAsync_AddsNewPopsicle()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();
        var newPopsicle = new PopsicleModel 
        {
            Name = "Mango Madness",
            Description = "Tropical mango flavor"
        };

        // Act
        var result = await repository.CreatePopsicleAsync(newPopsicle);

        // Assert
        Assert.Equal(4, result.Id); // Repository starts with 3 items, next ID is 4
        Assert.Equal("Mango Madness", result.Name);

        var allPopsicles = await repository.GetAllPopsicles();
        Assert.Equal(4, allPopsicles.Count());
    }

    [Fact]
    public async Task UpdatePopsicleAsync_WithExistingPopsicle_UpdatesSuccessfully()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();
        var updatedPopsicle = new PopsicleModel
        { 
            Id = 1, 
            Name = "Updated Strawberry", 
            Description = "Updated description" 
        };

        // Act
        var result = await repository.UpdatePopsicleAsync(updatedPopsicle);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Strawberry", result.Name);
    }

    [Fact]
    public async Task DeletePopsicleAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.DeletePopsicleAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeletePopsicleAsync_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.DeletePopsicleAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchPopsiclesAsync_WithMatchingName_ReturnsResults()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.SearchPopsiclesAsync("Strawberry");

        // Assert
        var resultsList = result.ToList(); // Convert to list to prevent multiple enumeration
        Assert.Single(resultsList);
        Assert.Equal("Strawberry Delight", resultsList.First().Name);
    }

    [Fact]
    public async Task SearchPopsiclesAsync_WithNoMatches_ReturnsEmptyResult()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.SearchPopsiclesAsync("chocolate");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchPopsiclesAsync_WithEmptySearchTerm_ReturnsAllPopsicles()
    {
        // Arrange
        var repository = new InMemoryInventoryRepository();

        // Act
        var result = await repository.SearchPopsiclesAsync("");

        // Assert
        Assert.Equal(3, result.Count()); // Should return all 3 default popsicles
    }
}