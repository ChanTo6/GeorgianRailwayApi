using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GeorgianRailwayApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GeorgianRailwayApi.Data;
using MediatR;
using GeorgianRailwayApi.Features.Auth.Register;
using GeorgianRailwayApi.Features.Auth.Login;
using GeorgianRailwayApi.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace GeorgianRailwayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;
        public AuthController(IMediator mediator, IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var errors = ValidateRegister(dto);
            if (errors.Count > 0)
                return BadRequest(ApiErrorResponse.Validation("Validation failed", errors));

            var command = new RegisterCommand { Email = dto.Email, Password = dto.Password, Role = dto.Role };
            var result = await _mediator.Send(command);
            if (result == null)
                return BadRequest(ApiErrorResponse.Failure("Registration failed", "Email already exists.", "EmailExists"));

            _cache.Remove("train_list");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var errors = ValidateLogin(dto);
            if (errors.Count > 0)
                return BadRequest(ApiErrorResponse.Validation("Validation failed", errors));

            var command = new LoginCommand { Email = dto.Email, Password = dto.Password };
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized(ApiErrorResponse.Failure("Login failed", "Invalid email or password.", "InvalidCredentials", StatusCodes.Status401Unauthorized));

            return Ok(result);
        }

        [HttpPost("verify-pin")]
        public async Task<IActionResult> VerifyPin([FromBody] VerifyPinRequestDto dto, [FromServices] ApplicationDbContext db)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(ApiErrorResponse.Failure("Verification failed", "User not found.", "UserNotFound", StatusCodes.Status404NotFound));
            if (user.IsVerified)
                return BadRequest(ApiErrorResponse.Failure("Verification failed", "User already verified.", "AlreadyVerified"));
            if (user.VerificationPin != dto.Pin)
                return BadRequest(ApiErrorResponse.Failure("Verification failed", "Invalid PIN code.", "InvalidPin"));

            user.IsVerified = true;
            user.VerificationPin = null;
            await db.SaveChangesAsync();
            return Ok(new { message = "Account verified successfully." });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            if (email == null || idClaim == null)
            {
                return Unauthorized(ApiErrorResponse.Failure("Unauthorized", "User not found in token.", "UserNotFoundInToken", StatusCodes.Status401Unauthorized));
            }

            return Ok(new
            {
                Id = int.TryParse(idClaim, out var id) ? id : 0,
                Email = email,
                Role = role
            });
        }

<<<<<<< HEAD
     
=======
        
>>>>>>> 73621f9 (final)
        private static List<string> ValidateRegister(RegisterRequestDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Email format is invalid.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");
            else if (dto.Password.Length < 6)
                errors.Add("Password must be at least 6 characters.");
            return errors;
        }

        private static List<string> ValidateLogin(LoginRequestDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Email format is invalid.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");
            else if (dto.Password.Length < 6)
                errors.Add("Password must be at least 6 characters.");
            return errors;
        }
    }
}
