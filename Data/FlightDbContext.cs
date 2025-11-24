using FlightInformationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightInformationApi.Data;

public class FlightDbContext : DbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options)
    {
    }

    public DbSet<Flight> Flights { get; set; } = null!;
}
