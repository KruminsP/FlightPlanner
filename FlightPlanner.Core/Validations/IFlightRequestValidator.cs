namespace FlightPlanner.Core.Validations
{
    public interface IFlightRequestValidator
    {
        bool IsValid(SearchFlightsRequest req);
    }
}