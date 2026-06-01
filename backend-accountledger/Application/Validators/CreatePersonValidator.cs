using BackendAccountLedger.Application.DTOs;
using FluentValidation;

namespace BackendAccountLedger.Application.Validators;

public class CreatePersonValidator : AbstractValidator<CreatePersonDto>
{
    public CreatePersonValidator() 
    {
        RuleFor(x => x.id_number)
            .NotEmpty().WithMessage("ID Number is required.")
            .MaximumLength(20).WithMessage("ID Number cannot exceed 20 characters.")
            .Matches(@"^\d{10,13}$").WithMessage("ID Number must be 10-13 digits.");

        RuleFor(x => x.first_name)
            .NotEmpty().WithMessage("First Name is required.")
            .MaximumLength(50);

        RuleFor(x => x.surname)
            .NotEmpty().WithMessage("Surname is required.")
            .MaximumLength(50);

        RuleFor(x => x.contact_info)
            .MaximumLength(100);
    }
}