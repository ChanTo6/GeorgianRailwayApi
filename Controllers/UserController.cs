using AutoMapper;
using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.DTOs;
using GeorgianRailwayApi.Features.UserPanel.SoldTickets;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeorgianRailwayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;

        public UserController(IMediator mediator, IMapper mapper, IMemoryCache cache, ApplicationDbContext context)
        {
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
            _context = context;
        }

        [HttpGet("all-trains")]
        public async Task<IActionResult> GetAllTrains()
        {
            if (!_cache.TryGetValue("train_list", out List<TrainResponseDto> dtoList))
            {
                var result = await _mediator.Send(new Features.UserPanel.GetAllTrains.Query());
                dtoList = _mapper.Map<List<TrainResponseDto>>(result);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                _cache.Set("train_list", dtoList, cacheEntryOptions);
            }
            return Ok(dtoList);
        }

        [HttpGet("search-trains")]
        public async Task<IActionResult> SearchTrains([FromQuery] string source, [FromQuery] string destination)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(source))
                errors.Add("Source is required.");
            if (string.IsNullOrWhiteSpace(destination))
                errors.Add("Destination is required.");
            if (errors.Count > 0)
                return BadRequest(ApiErrorResponse.Validation("Validation failed", errors));

            var result = await _mediator.Send(new Features.UserPanel.SearchTrainsInfo.SearchTrainsQuery.Query { Source = source, Destination = destination });
            var dtoList = _mapper.Map<List<TrainResponseDto>>(result);
            return Ok(dtoList);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyTickets([FromBody] TicketPurchaseRequestDto request)
        {
            var errors = ValidateTicketPurchase(request);
            if (errors.Count > 0)
                return BadRequest(ApiErrorResponse.Validation("Validation failed", errors));

            var command = new Features.UserPanel.BuyTickets.BuyTicketsCommand
            {
                UserId = request.UserId,
                Tickets = request.Tickets
            };
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(ApiErrorResponse.Failure("Ticket purchase failed", result.Message, "TicketPurchaseFailed"));

            _cache.Remove("train_list");
            return Ok(result);
        }

        [HttpGet("sold-tickets/{userId}")]
        public async Task<IActionResult> GetSoldTicketsByUserId(int userId)
        {
            var dtos = await _mediator.Send(new GetSoldTicketsByUserIdQuery { UserId = userId });
            if (dtos == null || !dtos.Any())
                return NotFound(ApiErrorResponse.Failure("Not found", $"No sold tickets for user {userId}.", "SoldTicketsNotFound", StatusCodes.Status404NotFound));

            return Ok(dtos);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiErrorResponse.Failure("Unauthorized", "User is not logged in.", "Unauthorized", StatusCodes.Status401Unauthorized));

            var dto = await _mediator.Send(new Features.UserPanel.Profile.GetProfileQuery { UserId = userId });
            if (dto == null)
                return NotFound(ApiErrorResponse.Failure("Not found", "User not found.", "UserNotFound", StatusCodes.Status404NotFound));
            return Ok(dto);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] DTOs.UpdateRequestDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiErrorResponse.Failure("Unauthorized", "User is not logged in.", "Unauthorized", StatusCodes.Status401Unauthorized));
            if (dto.Id != userId)
                return Forbid();

            var command = new Features.UserPanel.Profile.UpdateProfileCommand
            {
                Id = userId,
                Email = dto.Email, // Only update if provided
                Password = dto.Password // Only update if provided
            };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(ApiErrorResponse.Failure("Not found", "User not found.", "UserNotFound", StatusCodes.Status404NotFound));
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(ApiErrorResponse.Failure("Unauthorized", "User is not logged in.", "Unauthorized", StatusCodes.Status401Unauthorized));

            var command = new Features.UserPanel.Profile.DeleteProfileCommand { Id = userId };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(ApiErrorResponse.Failure("Not found", "User not found.", "UserNotFound", StatusCodes.Status404NotFound));
            return Ok(new { message = "User deleted successfully." });
        }

        private static List<string> ValidateTicketPurchase(TicketPurchaseRequestDto request)
        {
            var errors = new List<string>();
            if (request == null)
            {
                errors.Add("Request body is required.");
                return errors;
            }
            if (request.UserId <= 0)
                errors.Add("UserId is required and must be greater than zero.");
            if (request.Tickets == null || request.Tickets.Count == 0)
                errors.Add("At least one ticket must be provided.");
            return errors;
        }
    }
}

