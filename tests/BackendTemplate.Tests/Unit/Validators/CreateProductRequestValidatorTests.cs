using BackendTemplate.Application.DTOs;
using BackendTemplate.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BackendTemplate.Tests.Unit.Validators;

public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator;

    public CreateProductRequestValidatorTests()
    {
        _validator = new CreateProductRequestValidator();
    }

    [Fact]
    public void Validate_WhenRequestIsValid_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Valid Product",
            Description = "Valid Description",
            Price = 10.0m,
            Stock = 5,
            CategoryId = 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_WhenNameIsEmpty_ShouldHaveError(string name)
    {
        // Arrange
        var request = new CreateProductRequest { Name = name };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenPriceIsZeroOrNegative_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Price = 0 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_WhenStockIsNegative_ShouldHaveError()
    {
        // Arrange
        var request = new CreateProductRequest { Stock = -1 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }
}
