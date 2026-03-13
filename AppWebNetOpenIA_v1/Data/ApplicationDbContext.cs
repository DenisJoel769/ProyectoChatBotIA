using AppWebNetOpenIA_v1.Models;
using AppWebNetOpenIA_v1.Models.Views;
using Microsoft.EntityFrameworkCore;

namespace AppWebNetOpenIA_v1.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VentasDetalleView>()
                .HasNoKey()
                .ToView("v_ventas_detalles_v1"); //apunta a la vista 

            modelBuilder.Entity<ComprasDetalleView>()
                .HasNoKey()
                .ToView("v_compra_detalle_v1"); //apunta a la vista 
            modelBuilder.Entity<StockView>()
                .HasNoKey()
                .ToView("v_stock");
        }
        public DbSet<VentasDetalleView> VentasDetalles { get; set; }
        public DbSet<ComprasDetalleView> ComprasDetalles { get; set; }
        public DbSet<StockView> Stocks { get; set; }

        public DbSet<Usuario> usuarios { get; set; }
    }
}
