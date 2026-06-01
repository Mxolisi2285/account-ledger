using BackendAccountLedger.Application.DTOs;
using FluentValidation;

namespace BackendAccountLedger.Application.Validators;

public class UpdateAccountValidator : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountValidator()
    {
        RuleFor(x => x.account_number).NotEmpty().WithMessage("Account Number is required.").MaximumLength(20);
    }
}