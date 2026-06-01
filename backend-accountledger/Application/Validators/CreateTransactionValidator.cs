using BackendAccountLedger.Application.DTOs;
using FluentValidation;

namespace BackendAccountLedger.Application.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.account_id).GreaterThan(0).WithMessage("Valid Account ID is required.");
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