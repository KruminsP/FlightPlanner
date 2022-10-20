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
        private static readonly object _lock = new object();

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
            lock (_lock)
            {
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
            }
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            lock (_lock)
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
            }

            return Ok();
        }
    }
}
