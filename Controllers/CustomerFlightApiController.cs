using FlightPlanner.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomerFlightApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;

        public CustomerFlightApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("airports")]
        [HttpGet]
        public IActionResult SearchAirports(string search)
        {
            search = search.Trim().ToLower();
            var searchResult = _context.Airports.Where(a =>
                a.AirportCode.ToLower().Contains(search) ||
                a.City.ToLower().Contains(search) ||
                a.Country.ToLower().Contains(search));
            return Ok(searchResult);
        }

        [Route("flights/search")]
        [HttpPost]
        public IActionResult SearchFlights(SearchFlightsRequest req)
        {
            if (FlightRequestValidator.IsRequestInvalid(req) ||
                FlightRequestValidator.IsFromAndToAirportTheSame(req))
            {
                return BadRequest();
            }

            List<Flight> flights;
            flights = _context.Flights
                .Include(flight => flight.From)
                .Include(flight => flight.To)
                .ToList().Where(flight =>
                    flight.From.AirportCode == req.From &&
                    flight.To.AirportCode == req.To &&
                    DateTime.Parse(flight.DepartureTime).Date == DateTime.Parse(req.DepartureDate).Date).ToList();
            var pageResult = new PageResult(flights);
            return Ok(pageResult);
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult FindFlightById(int id)
        {
            Flight flight;
            flight = _context.Flights
                .Include(flight => flight.From)
                .Include(flight => flight.To)
                .FirstOrDefault(flight => flight.Id == id);
            return flight == null ? NotFound($"No flight with id {id}") : Ok(flight);
        }
    }
}
