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

        
        private static List<string> ValidateTrain(TrainRequestDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Name))
                errors.Add("Train name is required.");
            if (string.IsNullOrWhiteSpace(dto.Source))
                errors.Add("Source is required.");
            if (string.IsNullOrWhiteSpace(dto.Destination))
                errors.Add("Destination is required.");
            if (dto.TotalSeats <= 0)
                errors.Add("Total seats must be greater than zero.");
            if (dto.Date == default)
                errors.Add("Date is required.");
            if (string.IsNullOrWhiteSpace(dto.Time))
                errors.Add("Time is required.");
            return errors;
        }
    }
}