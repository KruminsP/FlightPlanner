namespace FlightPlanner.Core.Validations
{
    public class AirportCountryValidator : IAirportValidator
    {
        public bool IsValid(Airport airport)
        {
            return !string.IsNullOrEmpty(airport?.Country);
        }
    }
}