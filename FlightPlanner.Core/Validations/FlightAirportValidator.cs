namespace FlightPlanner.Core.Validations
{
    public class FlightAirportValidator : IFlightValidator
    {
        public bool IsValid(Flight flight)
        {
            if (flight.From != null && flight.To != null)
            {
                return flight.From.AirportCode?.Trim().ToLower() !=
                       flight.To.AirportCode?.Trim().ToLower();
            }

            return false;
        }
    }
}