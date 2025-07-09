using FluentValidation;
using GeorgianRailwayApi.DTOs;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
