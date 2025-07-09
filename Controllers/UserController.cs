using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using GeorgianRailwayApi.DTOs;

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

        [HttpGet("sold-ticket/{id}")]
        public async Task<IActionResult> GetSoldTicketById(int id)
        {
            var dto = await _mediator.Send(new Features.UserPanel.SoldTickets.GetSoldTicketByIdQuery { TicketId = id });
            if (dto == null)
                return NotFound(ApiErrorResponse.Failure("Not found", $"Sold ticket with id {id} not found.", "SoldTicketNotFound", StatusCodes.Status404NotFound));
            return Ok(dto);
        }

        // --- Private helpers ---
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

