using GeorgianRailwayApi.Data;
using GeorgianRailwayApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static GeorgianRailwayApi.Features.UserPanel.BookSeatInfo.BookSeatCommand;
using static GeorgianRailwayApi.Features.UserPanel.SearchTrainsInfo.SearchTrainsQuery;
using GeorgianRailwayApi.Features.UserPanel.BuyTickets;
using GeorgianRailwayApi.DTOs;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System;
using GeorgianRailwayApi.Features;

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
                var result = await _mediator.Send(new GetAllTrains.Query());
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
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Invalid search parameters",
                    Detail = "Source and Destination must be provided.",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400"
                };
                problemDetails.Extensions["errorCode"] = "MissingSearchParams";
                return BadRequest(problemDetails);
            }

            var result = await _mediator.Send(new Features.UserPanel.SearchTrainsInfo.SearchTrainsQuery.Query { Source = source, Destination = destination });
            var dtoList = _mapper.Map<List<TrainResponseDto>>(result);
            return Ok(dtoList);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyTickets([FromBody] TicketPurchaseRequestDto request)
        {
            var command = new Features.UserPanel.BuyTickets.BuyTicketsCommand
            {
                UserId = request.UserId,
                Tickets = request.Tickets // Already TicketPurchaseDto
            };
            var result = await _mediator.Send(command);
            if (!result.Success)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Ticket purchase failed",
                    Detail = result.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400"
                };
                problemDetails.Extensions["errorCode"] = "TicketPurchaseFailed";
                return BadRequest(problemDetails);
            }
            // Invalidate train list cache after purchase
            _cache.Remove("train_list");
            return Ok(result);
        }

        [HttpGet("sold-ticket/{id}")]
        public async Task<IActionResult> GetSoldTicketById(int id)
        {
            var dto = await _mediator.Send(new GeorgianRailwayApi.Features.UserPanel.SoldTickets.GetSoldTicketByIdQuery { TicketId = id });
            if (dto == null)
                return NotFound(new { error = $"Sold ticket with id {id} not found." });
            return Ok(dto);
        }
    }
}

