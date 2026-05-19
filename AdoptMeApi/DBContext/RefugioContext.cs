using AdoptMeApi.Modelo;
using Microsoft.EntityFrameworkCore;

namespace AdoptMeApi.DBContext
{
    public class RefugioContext : DbContext
    {
        public RefugioContext(DbContextOptions<RefugioContext> options) : base(options) { }

        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Cuidador> Cuidadores { get; set; }
    }
}
