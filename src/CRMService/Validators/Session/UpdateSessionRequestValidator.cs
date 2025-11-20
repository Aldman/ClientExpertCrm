using CRMService.DTOs.Session;
using FluentValidation;

namespace CRMService.Validators.Session;

public class UpdateSessionRequestValidator : AbstractValidator<UpdateSessionRequestDto>
{
    public UpdateSessionRequestValidator()
    {
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.Now);
        RuleFor(x => x.DurationInMinutes).GreaterThan(0);
        RuleFor(x => x.Status)
            .Must(x => (int)x >= 0)
            .Must(x => (int)x <= 2)
            .WithMessage("Status should be between 0 and 2 (Planned, Completed, Canceled)");
    }
}