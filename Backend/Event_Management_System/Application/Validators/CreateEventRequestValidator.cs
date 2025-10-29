using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
    {
        public CreateEventRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Event title is required")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Start date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Cannot create event in the past");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
                .MinimumLength(3).WithMessage("Location must be at least 3 characters");

            RuleFor(x => x.Capacity)
                .GreaterThanOrEqualTo(0).WithMessage("Capacity cannot be negative");
         
            RuleFor(x => x.TagIds)
                .NotEmpty().WithMessage("At least one tag is required.")
                .Must(tags => tags.Count <= 5)
                .WithMessage("Maximum 5 tags are allowed per event.");



        }
    }
}