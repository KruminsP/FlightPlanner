using AutoMapper;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Validations;
using FlightPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerFlightApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnumerable<IFlightRequestValidator> _flightRequestValidators;
        private readonly IFlightService _flightService;

        public CustomerFlightApiController(
            IMapper mapper,
            IFlightService flightService,
            IEnumerable<IFlightRequestValidator> flightRequestValidator)
        {
            _mapper = mapper;
            _flightService = flightService;
            _flightRequestValidators = flightRequestValidator;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            var airports = _flightService.SearchAirport(search);
            var returnedAirports = new List<AirportRequest>();
            foreach (var airport in airports)
            {
                returnedAirports.Add(_mapper.Map<AirportRequest>(airport));
            }

            return Ok(returnedAirports);
        }

        [Route("flights/search")]
        [HttpPost]
        public IActionResult SearchFlights(SearchFlightsRequest req)
        {
            if (!_flightRequestValidators.All(f => f.IsValid(req)))
            {
                return BadRequest();
            }

            var flights = _flightService.GetRequestedFlights(req);
            var pageResult = new PageResult(flights);

            return Ok(pageResult);
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult FindFlightById(int id)
        {
            var flight = _flightService.GetCompleteFlightById(id);
            if (flight == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<FlightRequest>(flight);

            return Ok(response);
        }
    }
}
