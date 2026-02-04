using Microsoft.EntityFrameworkCore;
using Veterinaria_AppWeb.Models;
using System.Collections.Generic;

namespace Veterinaria_AppWeb.Data
{
    public class VeterinariaContext : DbContext
    {
        public VeterinariaContext(DbContextOptions<VeterinariaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Servicio> Servicio { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }


    }
}
