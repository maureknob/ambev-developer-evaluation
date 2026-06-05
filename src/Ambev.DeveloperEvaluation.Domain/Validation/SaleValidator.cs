using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(s => s.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required.")
            .MaximumLength(50).WithMessage("Sale number cannot exceed 50 characters.");

        RuleFor(s => s.SaleDate)
            .NotEmpty().WithMessage("Sale date is required.");

        RuleFor(s => s.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(s => s.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(s => s.BranchId)
            .NotEmpty().WithMessage("Branch ID is required.");

        RuleFor(s => s.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(100).WithMessage("Branch name cannot exceed 100 characters.");

        RuleFor(s => s.Items)
            .NotEmpty().WithMessage("Sale must contain at least one item.");

        RuleFor(s => s.Items)
            .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count)
            .WithMessage("Sale cannot contain duplicate products.");

        RuleForEach(s => s.Items).SetValidator(new SaleItemValidator());
    }
}

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(i => i.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(i => i.Quantity)
            .InclusiveBetween(1, 20).WithMessage("Quantity must be between 1 and 20.");

        RuleFor(i => i.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}
