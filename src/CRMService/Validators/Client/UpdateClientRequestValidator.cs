using CRMService.DTOs.Client;
using FluentValidation;

namespace CRMService.Validators.Client;

public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequestDto>
{
    public UpdateClientRequestValidator()
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