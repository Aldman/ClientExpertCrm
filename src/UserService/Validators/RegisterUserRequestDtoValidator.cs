using FluentValidation;
using UserService.DTOs;

namespace UserService.Validators;

public class RegisterUserRequestDtoValidator : AbstractValidator<RegisterUserRequestDto>
{
    public RegisterUserRequestDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).NotNull().NotEmpty();
    }
}