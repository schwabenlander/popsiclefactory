using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PopsicleFactory.Api.Controllers;
using PopsicleFactory.Api.Models;
using PopsicleFactory.Api.Repositories;

namespace PopsicleFactory.Tests;

public class InventoryControllerTests
{
    private readonly IInventoryRepository _mockRepository;
    private readonly IValidator<PopsicleModel> _mockValidator;
    private readonly InventoryController _controller;

    public InventoryControllerTests()
    {
        _mockRepository = Substitute.For<IInventoryRepository>();
        _mockValidator = Substitute.For<IValidator<PopsicleModel>>();
        _controller = new InventoryController(_mockRepository, _mockValidator);
    }

    [Fact]
    public async Task GetAllPopsicles_ReturnsOkWithAllPopsicles()
    {
        // Arrange
        var popsicles = new List<PopsicleModel>
        {
            new() { Id = 1, Name = "Popsicle 1", Description = "Description 1" },
            new() { Id = 2, Name = "Popsicle 2", Description = "Description 2" }
        };

        _mockRepository.GetAllPopsicles().Returns(popsicles);

        // Act
        var result = await _controller.GetAllPopsicles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(popsicles, okResult.Value);
    }
    
    [Fact]
    public async Task GetPopsicle_WithExistingId_ReturnsOkResult()
    {
        // Arrange
        var popsicle = new PopsicleModel { Id = 1, Name = "Test Popsicle", Description = "Test Description" };
        _mockRepository.GetPopsicleByIdAsync(1).Returns(popsicle);

        // Act
        var result = await _controller.GetPopsicle(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(popsicle, okResult.Value);
    }

    [Fact]
    public async Task GetPopsicle_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.GetPopsicleByIdAsync(999).Returns((PopsicleModel?)null);

        // Act
        var result = await _controller.GetPopsicle(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreatePopsicle_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var popsicle = new PopsicleModel { Name = "New Popsicle", Description = "New Description" };
        var createdPopsicle = new PopsicleModel { Id = 1, Name = "New Popsicle", Description = "New Description" };
        var validationResult = new ValidationResult(); // Empty validation result means valid

        _mockValidator.ValidateAsync(popsicle).Returns(validationResult);
        _mockRepository.CreatePopsicleAsync(popsicle).Returns(createdPopsicle);

        // Act
        var result = await _controller.CreatePopsicle(popsicle);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(InventoryController.GetPopsicle), createdResult.ActionName);
        Assert.Equal(createdPopsicle, createdResult.Value);
    }

    [Fact]
    public async Task CreatePopsicle_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var popsicle = new PopsicleModel { Name = "", Description = "" };
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Name", 
            "Name is required") });

        _mockValidator.ValidateAsync(popsicle).Returns(validationResult);

        // Act
        var result = await _controller.CreatePopsicle(popsicle);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(validationResult.Errors, badRequestResult.Value);
    }

    [Fact]
    public async Task UpdatePopsicle_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var popsicle = new PopsicleModel { Id = 1, Name = "Updated Popsicle", Description = "Updated Description" };
        var existingPopsicle = new PopsicleModel { Id = 1, Name = "Old Popsicle", Description = "Old Description" };
        var validationResult = new ValidationResult(); // Empty validation result = valid

        _mockValidator.ValidateAsync(popsicle).Returns(validationResult);
        _mockRepository.GetPopsicleByIdAsync(1).Returns(existingPopsicle);
        _mockRepository.UpdatePopsicleAsync(popsicle).Returns(popsicle);

        // Act
        var result = await _controller.UpdatePopsicle(1, popsicle);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(popsicle, okResult.Value);
    }

    [Fact]
    public async Task UpdatePopsicle_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var popsicle = new PopsicleModel { Id = 2, Name = "Updated Popsicle", Description = "Updated Description" };

        // Act
        var result = await _controller.UpdatePopsicle(1, popsicle); // URL ID = 1, Model ID = 2

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID in URL does not match ID in request body", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdatePopsicle_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var popsicle = new PopsicleModel { Id = 999, Name = "Updated Popsicle", Description = "Updated Description" };
        var validationResult = new ValidationResult(); // Empty validation result means valid

        _mockValidator.ValidateAsync(popsicle).Returns(validationResult);
        _mockRepository.GetPopsicleByIdAsync(999).Returns((PopsicleModel?)null);

        // Act
        var result = await _controller.UpdatePopsicle(999, popsicle);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeletePopsicle_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        _mockRepository.DeletePopsicleAsync(1).Returns(true);

        // Act
        var result = await _controller.DeletePopsicle(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePopsicle_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.DeletePopsicleAsync(999).Returns(false);

        // Act
        var result = await _controller.DeletePopsicle(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task SearchPopsicles_WithValidSearchTerm_ReturnsOkWithResults()
    {
        // Arrange
        var searchTerm = "strawberry";
        var popsicles = new List<PopsicleModel>
        {
            new() { Id = 1, Name = "Strawberry Delight", Description = "Fresh strawberry popsicle" }
        };

        _mockRepository.SearchPopsiclesAsync(searchTerm).Returns(popsicles);

        // Act
        var result = await _controller.SearchPopsicles(searchTerm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(popsicles, okResult.Value);
    }

    [Fact]
    public async Task SearchPopsicles_WithEmptySearchTerm_ReturnsBadRequest()
    {
        // Arrange - No setup needed for this test

        // Act
        var result = await _controller.SearchPopsicles("");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Search term is required", badRequestResult.Value);
    }

    [Fact]
    public async Task SearchPopsicles_WithNullSearchTerm_ReturnsBadRequest()
    {
        // Arrange - No setup needed for this test

        // Act
        var result = await _controller.SearchPopsicles(null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Search term is required", badRequestResult.Value);
    }
}