namespace FlightPlanner.Core.Validations
{
    public class HasDifferentAirports : IFlightRequestValidator
    {
        public bool IsValid(SearchFlightsRequest req)
        {
            return req.From != req.To;
        }
    }
}