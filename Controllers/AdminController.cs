using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeorgianRailwayApi.Models;
using GeorgianRailwayApi.Data;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GeorgianRailwayApi.DTOs;
using AutoMapper;
using GeorgianRailwayApi.Features.Admin.SoldTickets;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System;

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
        public AdminController(ApplicationDbContext context, IMediator mediator, IMapper mapper, IMemoryCache cache)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _cache = cache;
        }

        // Admin: Add new train
        [Authorize(Roles = "Admin")]
        [HttpPost("add-train")]
        public async Task<IActionResult> AddTrain([FromBody] TrainRequestDto trainDto)
        {
            if (string.IsNullOrWhiteSpace(trainDto.Name) || string.IsNullOrWhiteSpace(trainDto.Source) || string.IsNullOrWhiteSpace(trainDto.Destination))
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Invalid train data",
                    Detail = "Train name, source, and destination are required.",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://httpstatuses.com/400"
                };
                problemDetails.Extensions["errorCode"] = "InvalidTrainData";
                return BadRequest(problemDetails);
            }
            // Use MediatR to send a command to add a train
            var command = new Features.AdminPanel.AddTrain.AddTrainCommand { TrainDto = trainDto };
            var responseDto = await _mediator.Send(command);
            _cache.Remove("train_list");
            return Ok(responseDto);
        }

        // Admin: View all bookings (report) via CQRS/MediatR and AutoMapper
        [Authorize(Roles = "Admin")]
        [HttpGet("sold-tickets")]
        public async Task<IActionResult> GetSoldTickets()
        {
            // Do not cache sold tickets (dynamic data)
            var soldTickets = await _mediator.Send(new GetSoldTicketsQuery());
            return Ok(soldTickets);
        }
    }
}