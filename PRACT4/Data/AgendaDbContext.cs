using Microsoft.EntityFrameworkCore;
using PRACT4.Models;

namespace PRACT4.Data
{
    public class AgendaDbContext : DbContext
    {
        public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options)
        {
        }

        public DbSet<Estado> Estados { get; set; }
        public DbSet<Evento> Eventos { get; set; }
    }
}
