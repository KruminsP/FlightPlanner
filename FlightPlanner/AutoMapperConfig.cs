using AutoMapper;
using FlightPlanner.Models;

namespace FlightPlanner
{
    public class AutoMapperConfig
    {
        public static IMapper CreateMaper()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<AirportRequest, Airport>()
                        .ForMember(d => d.Id,
                            options => options.Ignore()
                        ).ForMember(d => d.AirportCode,
                            opt => opt.MapFrom(s=>s.Airport));
                    cfg.CreateMap<Airport, AirportRequest>()
                        .ForMember(d=>d.Airport,
                            opt =>opt.MapFrom(s=> s.AirportCode));
                    cfg.CreateMap<FlightRequest, Flight>();
                    cfg.CreateMap<Flight, FlightRequest>();
                });
            config.AssertConfigurationIsValid(); //delete after testing if correct

            return config.CreateMapper();
        }
    }
}