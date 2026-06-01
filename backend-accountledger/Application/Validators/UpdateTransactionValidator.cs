using BackendAccountLedger.Application.DTOs;
using FluentValidation;

namespace BackendAccountLedger.Application.Validators;

public class UpdateTransactionValidator : AbstractValidator<UpdateTransactionDto>
{
    public UpdateTransactionValidator()
    {
        RuleFor(x => x.transaction_date)
            .NotEmpty().WithMessage("Transaction Date is required.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Transaction Date cannot be in the future.");
        RuleFor(x => x.amount)
            .NotEmpty().WithMessage("Amount is required.")
            .NotEqual(0m).WithMessage("Transaction amount cannot be zero.")
            .GreaterThan(0m).WithMessage("Amount must be greater than 0.");
        RuleFor(x => x.type)
            .NotEmpty().WithMessage("Transaction Type is required.")
            .Must(t => t == "Debit" || t == "Credit").WithMessage("Type must be 'Debit' or 'Credit'.");
    }
}