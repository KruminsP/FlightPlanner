using System;
using System.Linq;
using FlightPlanner.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers
{
    [Route("admin-api")]
    [ApiController, Authorize]
    public class AdminApiController : ControllerBase
    {
        private readonly FlightPlannerDbContext _context;
        private static readonly object _lock = new object();

        public AdminApiController(FlightPlannerDbContext context)
        {
            _context = context;
        }

        [Route("flights/{id}")]
        [HttpGet]
        public IActionResult GetFlight(int id)
        {
            var flight = _context.Flights
                .Include(flight => flight.From)
                .Include(flight => flight.To)
                .FirstOrDefault(flight => flight.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        [Route("flights")]
        [HttpPut]
        public IActionResult PutFlight(Flight flight)
        {
            try
            {
                FlightValidator.Validator(flight);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            if (flight.From.AirportCode.ToLower().Trim() == flight.To.AirportCode.ToLower().Trim() ||
                TimeValidator.Validator(flight))
            {
                return BadRequest();
            }

            lock (_lock)
            {
                if (_context.Flights
                    .Include(flight => flight.From)
                    .Include(flight => flight.To)
                    .Any(searchFlight => searchFlight.ArrivalTime == flight.ArrivalTime &&
                                         searchFlight.DepartureTime == flight.DepartureTime &&
                                         searchFlight.Carrier == flight.Carrier &&
                                         searchFlight.From.AirportCode == flight.From.AirportCode &&
                                         searchFlight.To.AirportCode == flight.To.AirportCode))
                {
                    return Conflict("Flight already exists");
                }

                _context.Flights.Add(flight);
                _context.SaveChanges();
                return Created("Flight created", flight);
            }
        }

        [Route("flights/{id}")]
        [HttpDelete]
        public IActionResult DeleteFlight(int id)
        {
            lock (_lock)
            {
                var flight = _context.Flights
                    .Include(flight => flight.From)
                    .Include(flight => flight.To)
                    .FirstOrDefault(flight => flight.Id == id);
                if (flight != null)
                {
                    _context.Flights.Remove(flight);
                    _context.SaveChanges();
                }
            }

            return Ok();
        }
    }
}
