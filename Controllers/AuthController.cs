using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GeorgianRailwayApi.Models;
using GeorgianRailwayApi.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GeorgianRailwayApi.Data;
using System.Security.Cryptography;
using System.Text;
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


        [HttpPost("verify-pin")]
        public async Task<IActionResult> VerifyPin([FromBody] VerifyPinRequestDto dto, [FromServices] GeorgianRailwayApi.Data.ApplicationDbContext db)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new { error = "User not found." });
            if (user.IsVerified)
                return BadRequest(new { error = "User already verified." });
            if (user.VerificationPin != dto.Pin)
                return BadRequest(new { error = "Invalid PIN code." });
            user.IsVerified = true;
            user.VerificationPin = null;
            await db.SaveChangesAsync();
            return Ok(new { message = "Account verified successfully." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var command = new RegisterCommand { Email = dto.Email, Password = dto.Password, Role = dto.Role };
            var result = await _mediator.Send(command);
            if (result == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Registration failed",
                    Detail = "Email already exists.",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400"
                };
                problemDetails.Extensions["errorCode"] = "EmailExists";
                return BadRequest(problemDetails);
            }
            // Invalidate train list cache in case admin registers
            _cache.Remove("train_list");
            return Ok(result);
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var command = new LoginCommand { Email = dto.Email, Password = dto.Password };
            var result = await _mediator.Send(command);
            if (result == null)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Login failed",
                    Detail = "Invalid email or password.",
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://httpstatuses.com/401"
                };
                problemDetails.Extensions["errorCode"] = "InvalidCredentials";
                return Unauthorized(problemDetails);
            }
            return Ok(result);
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
                return Unauthorized(new { error = "User not found in token." });
            }

            return Ok(new
            {
                Id = int.TryParse(idClaim, out var id) ? id : 0,
                Email = email,
                Role = role
            });
        }

      
    }
}
