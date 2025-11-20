using CRMService.DTOs.Client;
using FluentValidation;

namespace CRMService.Validators.Client;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequestDto>
{
    public CreateClientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .EmailAddress();
        RuleFor(x => x.Phone)
            .NotNull()
            .NotEmpty()
            .MaximumLength(20)
            .Must(phone => !phone.Any(char.IsLetter));
    }
}