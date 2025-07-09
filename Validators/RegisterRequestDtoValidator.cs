using FluentValidation;
using GeorgianRailwayApi.DTOs;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
