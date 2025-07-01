    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using ProyectoFinal_Santiago_Giustolisi.Models;
    using ProyectoFinal.Models;

    namespace ProyectoFinal_Santiago_Giustolisi.Data
    {
        public class ApplicationDbContext : IdentityDbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }
            public DbSet<ProyectoFinal_Santiago_Giustolisi.Models.StockProducto> StockProducto { get; set; } = default!;
            public DbSet<ProyectoFinal_Santiago_Giustolisi.Models.Categoria> Categoria { get; set; } = default!;
            public DbSet<ProyectoFinal_Santiago_Giustolisi.Models.Deudor> Deudor { get; set; } = default!;
            public DbSet<ProyectoFinal_Santiago_Giustolisi.Models.Ingreso> Ingreso { get; set; } = default!;
            public DbSet<ProyectoFinal.Models.Pedido> Pedido { get; set; } = default!;
            public DbSet<ProyectoFinal_Santiago_Giustolisi.Models.Proveedor> Proveedor { get; set; } = default!;
            public DbSet<ProyectoFinal_Santiago_Giustolisi.Models.Venta> Venta { get; set; } = default!;
        }

    }
