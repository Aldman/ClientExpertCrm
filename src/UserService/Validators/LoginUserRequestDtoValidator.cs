using FluentValidation;
using UserService.DTOs;

namespace UserService.Validators;

public class LoginUserRequestDtoValidator : AbstractValidator<LoginUserRequestDto>
{
    public LoginUserRequestDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).NotNull().NotEmpty();
    }
}