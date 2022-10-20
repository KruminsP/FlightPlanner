namespace FlightPlanner.Core.Validations
{
    public class FlightRequestValidator : IFlightRequestValidator
    {
        public bool IsValid(SearchFlightsRequest req)
        {
            return !string.IsNullOrEmpty(req.DepartureDate) ||
                   !string.IsNullOrEmpty(req.From) ||
                   !string.IsNullOrEmpty(req.To);
        }
    }
}