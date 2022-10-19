using AutoMapper;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using FlightPlanner.Core.Validations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FlightPlanner.Filters;
using Microsoft.EntityFrameworkCore;
using FlightPlanner.Data;
using FlightPlanner.Services;

namespace FlightPlanner
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FlightPlanner", Version = "v1" });
            });

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthorizationHandler>("BasicAuthentication", null);

            services.AddDbContext<FlightPlannerDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("flight-planner"));
            });

            // Default Policy
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:8080", "http://localhost:4200") //("https://localhost:44351", "http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddScoped<IFlightPlannerDbContext, FlightPlannerDbContext>();
            services.AddScoped<IDbService, DbService>();
            services.AddScoped<IEntityService<Airport>, EntityService<Airport>>();
            services.AddScoped<IEntityService<Flight>, EntityService<Flight>>();
            services.AddScoped<IEntityService<User>, EntityService<User>>();
            services.AddScoped<IFlightService, FlightService>();

            services.AddScoped<IFlightValidator, CarrierValidator>();
            services.AddScoped<IFlightValidator, FlightTimeValidator>();
            services.AddScoped<IFlightValidator, FlightAirportValidator>();
            services.AddScoped<IAirportValidator, AirportCityValidator>();
            services.AddScoped<IAirportValidator, AirportCountryValidator>();
            services.AddScoped<IAirportValidator, AirportCodeValidator>();
            services.AddScoped<IFlightRequestValidator, FlightRequestValidator>();
            services.AddScoped<IFlightRequestValidator, HasDifferentAirports>();

            services.AddSingleton<IMapper>(AutoMapperConfig.CreateMaper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightPlanner v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            

        }
    }
}
