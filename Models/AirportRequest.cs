namespace FlightPlanner.Models
{
    public class AirportRequest
    {
        public string Country { get; set; }
        public string City { get; set; }
        //[JsonPropertyName("airport")]
        public string Airport { get; set; }
    }
}