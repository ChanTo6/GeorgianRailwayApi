using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeorgianRailwayApi.Models;
using GeorgianRailwayApi.Data;
using System.Threading.Tasks;
using MediatR;

using GeorgianRailwayApi.DTOs;
using AutoMapper;

using Microsoft.Extensions.Caching.Memory;
using System;
using GeorgianRailwayApi.Features.Admin.SoldTickets;
using FluentValidation;
using GeorgianRailwayApi.Features.Auth.Register;

namespace GeorgianRailwayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {


        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IValidator<TrainRequestDto> _trainValidator;
        public AdminController(ApplicationDbContext context, IMediator mediator, IMapper mapper, IMemoryCache cache, IValidator<TrainRequestDto> trainValidator)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
            _trainValidator = trainValidator;
        }
        //var adminEmail = "admin@georgianrailway.local";
        //var adminPassword = "Admin@12345"


    // Admin: Add new train
    [Authorize(Roles = "Admin")]
        [HttpPost("add-train")]
        public async Task<IActionResult> AddTrain([FromBody] TrainRequestDto trainDto)
        {
            var validationResult = await _trainValidator.ValidateAsync(trainDto);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails();
                foreach (var error in validationResult.Errors)
                {
                    problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                }
                problemDetails.Title = "Validation Failed";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                return BadRequest(problemDetails);
            }
            // Use MediatR to send a command to add a train
            var command = new Features.AdminPanel.AddTrain.AddTrainCommand { TrainDto = trainDto };
            var responseDto = await _mediator.Send(command);
            _cache.Remove("train_list");
            return Ok(responseDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sold-tickets")]
        public async Task<IActionResult> GetSoldTickets()
        {
            var soldTickets = await _mediator.Send(new GetSoldTicketsQuery());
            return Ok(soldTickets);
        }

     
        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mediator.Send(new Features.AdminPanel.GetAllUsers.GetAllUsersQuery());
            return Ok(users);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] RegisterRequestDto dto)
        {
            var errors = ValidateRegister(dto);
            if (errors.Count > 0)
                return BadRequest(ApiErrorResponse.Validation("Validation failed", errors));

            var command = new RegisterCommand { Email = dto.Email, Password = dto.Password, Role = dto.Role };
            var result = await _mediator.Send(command);
            if (result == null)
                return BadRequest(ApiErrorResponse.Failure("Registration failed", "Email already exists.", "EmailExists"));

           
            return Ok(result);
        }

        // Admin: Update user
        [Authorize(Roles = "Admin")]
        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateRequestDto dto)
        {
            var command = new Features.AdminPanel.UpdateUser.UpdateUserCommand
            {
                Id = id,
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? null : dto.Role
            };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(ApiErrorResponse.Failure("Update failed", "User not found.", "UserNotFound"));
            return Ok(result);
        }



        // Admin: Delete user
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _mediator.Send(new Features.AdminPanel.DeleteUser.DeleteUserCommand { Id = id });
            if (!result)
                return NotFound(ApiErrorResponse.Failure("Delete failed", "User not found.", "UserNotFound"));
            return Ok(new { message = "User deleted successfully." });
        }

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
    }


}