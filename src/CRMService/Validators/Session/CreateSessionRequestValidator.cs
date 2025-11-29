using CRMService.DTOs.Session;
using FluentValidation;

namespace CRMService.Validators.Session;

public class CreateSessionRequestValidator : AbstractValidator<CreateSessionRequestDto>
{
    public CreateSessionRequestValidator()
    {
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.Now);
        RuleFor(x => x.DurationInMinutes).GreaterThan(0);
    }
}