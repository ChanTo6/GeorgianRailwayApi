using FluentValidation;
using GeorgianRailwayApi.DTOs;

public class TrainRequestDtoValidator : AbstractValidator<TrainRequestDto>
{
    public TrainRequestDtoValidator()
    {
        RuleFor(x => x.TrainId)
            .NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(name => !string.IsNullOrWhiteSpace(name));
        RuleFor(x => x.Source)
            .NotEmpty()
            .Must(src => !string.IsNullOrWhiteSpace(src));
        RuleFor(x => x.Destination)
            .NotEmpty()
            .Must(dest => !string.IsNullOrWhiteSpace(dest));
        RuleFor(x => x.Date)
            .NotEmpty();
        RuleFor(x => x.Time)
            .NotEmpty()
            .Must(time => !string.IsNullOrWhiteSpace(time));
        RuleFor(x => x.TotalSeats)
            .NotEmpty();
    }
}
