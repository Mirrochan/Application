using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
    {
        public UpdateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => x.Description != null);

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
                .MinimumLength(3).WithMessage("Location must be at least 3 characters")
                .When(x => !string.IsNullOrEmpty(x.Location));

            RuleFor(x => x.Capacity)
                .GreaterThanOrEqualTo(0).WithMessage("Capacity cannot be negative")
                .When(x => x.Capacity.HasValue);

            When(x => x.Date.HasValue, () =>
            {
                RuleFor(x => x.Date)
                    .GreaterThan(DateTime.UtcNow).WithMessage("Cannot set event in the past");
            });

            RuleFor(x => x.TagIds)
                .NotEmpty().WithMessage("At least one tag is required.")
                .Must(tags => tags.Count <= 5)
                .WithMessage("Maximum 5 tags are allowed per event.");
            
        }
    }
}
