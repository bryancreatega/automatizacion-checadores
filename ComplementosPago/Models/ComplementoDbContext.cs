using ComplementosPago.Models;
using Microsoft.EntityFrameworkCore;

public class ComplementoDbContext : DbContext
{
    public ComplementoDbContext(DbContextOptions<ComplementoDbContext> options)
        : base(options)
    {
    }

    public DbSet<ComplementoEnvio> ComplementoEnvios { get; set; }
    public DbSet<LecturaComplemento> LecturaComplementos { get; set; }
    public DbSet<DetalleLecturaComplemento> DetalleLecturaComplementos { get; set; }
    public DbSet<TiempoEjecucion> TiemposEjecucion { get; set; }
    public DbSet<LogEjecucion> LogsEjecucion { get; set; }


}
