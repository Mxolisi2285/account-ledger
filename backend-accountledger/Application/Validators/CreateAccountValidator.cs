using BackendAccountLedger.Application.DTOs;
using FluentValidation;

namespace BackendAccountLedger.Application.Validators;

public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
{
    public CreateAccountValidator()
    {
        // ✅ Removed async MustAsync rule — async validators break ASP.NET auto-validation
        // The account number uniqueness check is now handled in AccountService.CreateAsync
        RuleFor(x => x.person_id)
            .GreaterThan(0).WithMessage("Valid Person ID is required.");

        RuleFor(x => x.account_number)
            .NotEmpty().WithMessage("Account Number is required.")
            .MaximumLength(20).WithMessage("Account Number cannot exceed 20 characters.");
    }
}