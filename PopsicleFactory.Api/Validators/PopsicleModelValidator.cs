using FluentValidation;
using PopsicleFactory.Api.Models;

namespace PopsicleFactory.Api.Validators;

public class PopsicleModelValidator : AbstractValidator<PopsicleModel>
{
    public PopsicleModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");
    }
}