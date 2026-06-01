using BackendAccountLedger.Application.DTOs;
using FluentValidation;

namespace BackendAccountLedger.Application.Validators;

public class UpdatePersonValidator : AbstractValidator<UpdatePersonDto>
{
    public UpdatePersonValidator()
    {
        RuleFor(x => x.first_name).NotEmpty().WithMessage("First Name is required.").MaximumLength(50);
        RuleFor(x => x.surname).NotEmpty().WithMessage("Surname is required.").MaximumLength(50);
        RuleFor(x => x.contact_info).MaximumLength(100);
    }
}