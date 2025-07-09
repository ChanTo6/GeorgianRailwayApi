using FluentValidation;
using GeorgianRailwayApi.DTOs;

public class VerifyPinRequestDtoValidator : AbstractValidator<VerifyPinRequestDto>
{
    public VerifyPinRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        RuleFor(x => x.Pin)
            .NotEmpty()
            .Matches(@"^\d{6}$");
    }
}
