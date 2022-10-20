using System.Collections.Generic;

namespace FlightPlanner.Core.Services
{
    public interface IFlightService : IEntityService<Flight>
    {
        Flight GetCompleteFlightById(int id);
        public Airport[] SearchAirport(string search);
        bool Exists(Flight flight);
        List<Flight> GetRequestedFlights(SearchFlightsRequest req);
    }
}