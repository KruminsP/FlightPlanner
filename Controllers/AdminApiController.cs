using AutoMapper;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Validations;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController, Authorize, EnableCors("")]
    public class AdminApiController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IEnumerable<IFlightValidator> _flightValidators;
        private readonly IEnumerable<IAirportValidator> _airportValidators;
        private readonly IMapper _mapper;
        //private static readonly object _lock = new object();

        public AdminApiController(IFlightService flightService,
            IEnumerable<IFlightValidator> flightValidators,
            IEnumerable<IAirportValidator> airportValidators,
            IMapper mapper)
        {
            _flightService = flightService;
            _flightValidators = flightValidators;
            _airportValidators = airportValidators;
            _mapper = mapper;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = _flightService.GetCompleteFlightById(id);

            if (flight == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<FlightRequest>(flight);

            return Ok(response);
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(FlightRequest request)
        {
            var flight = _mapper.Map<Flight>(request);

            if (!_flightValidators.All(f => f.IsValid(flight)) ||
                !_airportValidators.All(f => f.IsValid(flight?.From)) ||
                !_airportValidators.All(f => f.IsValid(flight?.To)))
            {
                return BadRequest();
            }

            if (_flightService.Exists(flight))
            {
                return Conflict();
            }

            var result = _flightService.Create(flight);
            if (result.Success)
            {
                request = _mapper.Map<FlightRequest>(flight);

                return Created("", request);
            }

            return Problem(result.FormattedErrors);

            //try
            //{
            //    FlightValidator.Validator(request);
            //}
            //catch (Exception e)
            //{
            //    return BadRequest();
            //}

            //if (request.From.AirportCode.ToLower().Trim() == request.To.AirportCode.ToLower().Trim() ||
            //    TimeValidator.Validator(request))
            //{
            //    return BadRequest();
            //}

            //lock (_lock)
            //{
            //    if (_context.Flights
            //        .Include(request => request.From)
            //        .Include(request => request.To)
            //        .Any(searchFlight => searchFlight.ArrivalTime == request.ArrivalTime &&
            //                             searchFlight.DepartureTime == request.DepartureTime &&
            //                             searchFlight.Carrier == request.Carrier &&
            //                             searchFlight.From.AirportCode == request.From.AirportCode &&
            //                             searchFlight.To.AirportCode == request.To.AirportCode))
            //    {
            //        return Conflict("Flight already exists");
            //    }

            //    _context.Flights.Add(request);
            //    _context.SaveChanges();
            //    return Created("Flight created", request);
            //}
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _flightService.GetById(id);
            if (flight != null)
            {
                var result = _flightService.Delete(flight);
                if (result.Success)
                {
                    return Ok();
                }

                return Problem(result.FormattedErrors);
            }

            return Ok();

            //lock (_lock)
            //{
            //    var request = _context.Flights
            //        .Include(request => request.From)
            //        .Include(request => request.To)
            //        .FirstOrDefault(request => request.Id == id);
            //    if (request != null)
            //    {
            //        _context.Flights.Remove(request);
            //        _context.SaveChanges();
            //    }
            //}

            //return Ok();
        }
    }
}
