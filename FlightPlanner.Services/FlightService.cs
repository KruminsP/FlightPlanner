using FlightPlanner.Core.Services;
using FlightPlanner.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FlightPlanner.Services
{
    public class FlightService : EntityService<Flight>, IFlightService
    {
        public FlightService(IFlightPlannerDbContext context) : base(context)
        {
        }

        public Flight GetCompleteFlightById(int id)
        {
            return _context.Flights
                .Include(f => f.To)
                .Include(f => f.From)
                .SingleOrDefault(f => f.Id == id);
        }

        public Airport[] SearchAirport(string search)
        {
            search = search.Trim().ToLower();
            var airports = _context.Airports
                .Where(a =>
                    a.AirportCode.Contains(search) ||
                    a.Country.Contains(search) ||
                    a.City.Contains(search))
                .ToArray();

            return airports;
        }

        public bool Exists(Flight flight)
        {
            return _context.Flights.Any(f =>
                f.ArrivalTime == flight.ArrivalTime &&
                f.DepartureTime == flight.DepartureTime &&
                f.Carrier == flight.Carrier &&
                f.From.AirportCode == flight.From.AirportCode &&
                f.To.AirportCode == flight.To.AirportCode);
        }

        public List<Flight> GetRequestedFlights(SearchFlightsRequest req)
        {
            var flights = _context.Flights
                .Include(flight => flight.From)
                .Include(flight => flight.To)
                .Where(flight =>
                    flight.From.AirportCode == req.From &&
                    flight.To.AirportCode == req.To &&
                    flight.DepartureTime.Substring(0, 10) == req.DepartureDate).ToList();

            return flights;
        }
    }
}
