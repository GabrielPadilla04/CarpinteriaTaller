using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tablas Maestras y de Seguridad
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CodigoRecuperacion> CodigosRecuperacion { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<EstadoProyecto> EstadosProyecto { get; set; }
        public DbSet<EstadoTaller> EstadosTaller { get; set; } // Nuevo
        public DbSet<CategoriaMaterial> CategoriasMaterial { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; } // Nuevo

        // Tablas Operativas y de Proyecto
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<HistorialProyecto> HistorialProyectos { get; set; }
        public DbSet<VisitaEntrega> VisitasEntregas { get; set; }
        public DbSet<Ambiente> Ambientes { get; set; }
        public DbSet<Medicion> Mediciones { get; set; }

        // Catálogo e Inventario Intermedio
        public DbSet<Material> Materiales { get; set; }
        public DbSet<ProyectoMaterial> ProyectosMateriales { get; set; }
        public DbSet<ProveedorMaterial> ProveedoresMateriales { get; set; } // Nuevo

        // Auditoría y Notificaciones
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<BitacoraActividad> BitacoraActividades { get; set; }
        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo explícito a los nombres de tablas exactos en MariaDB (minúsculas y plurales)
            modelBuilder.Entity<Roles>().ToTable("roles");
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<CodigoRecuperacion>().ToTable("codigos_recuperacion");
            modelBuilder.Entity<Cliente>().ToTable("clientes");
            modelBuilder.Entity<EstadoProyecto>().ToTable("estados_proyecto");
            modelBuilder.Entity<EstadoTaller>().ToTable("estados_taller"); // Nuevo
            modelBuilder.Entity<CategoriaMaterial>().ToTable("categorias_material");
            modelBuilder.Entity<Proveedor>().ToTable("proveedores"); // Nuevo
            modelBuilder.Entity<Proyecto>().ToTable("proyectos");
            modelBuilder.Entity<HistorialProyecto>().ToTable("historial_proyectos");
            modelBuilder.Entity<VisitaEntrega>().ToTable("visitas_entregas");
            modelBuilder.Entity<Ambiente>().ToTable("ambientes");
            modelBuilder.Entity<Medicion>().ToTable("mediciones");
            modelBuilder.Entity<ProyectoMaterial>().ToTable("proyectos_materiales");
            modelBuilder.Entity<ProveedorMaterial>().ToTable("proveedores_materiales"); // Nuevo
            modelBuilder.Entity<Notificacion>().ToTable("notificaciones");
            modelBuilder.Entity<BitacoraActividad>().ToTable("bitacora_actividades");

            // Materiales y su relación explícita con la clave foránea categoria_id
            modelBuilder.Entity<Material>().ToTable("materiales");
            modelBuilder.Entity<Material>()
                .HasOne(m => m.CategoriaMaterial)
                .WithMany()
                .HasForeignKey(m => m.CategoriaId);

            // Índice único para el Email del usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Índice único para el CUIT del Proveedor
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.Cuit)
                .IsUnique();

            // Índice único para el Nombre del Estado de Taller
            modelBuilder.Entity<EstadoTaller>()
                .HasIndex(e => e.NombreEstadoTaller)
                .IsUnique();

            // Configuración de la clave primaria compuesta y relaciones para la tabla intermedia proyectos_materiales
            modelBuilder.Entity<ProyectoMaterial>()
                .HasKey(pm => new { pm.ProyectoId, pm.MaterialId });

            modelBuilder.Entity<ProyectoMaterial>()
                .HasOne(pm => pm.Proyecto)
                .WithMany()
                .HasForeignKey(pm => pm.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProyectoMaterial>()
                .HasOne(pm => pm.Material)
                .WithMany()
                .HasForeignKey(pm => pm.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la clave primaria compuesta y relaciones para la tabla intermedia proveedores_materiales (Nuevo)
            modelBuilder.Entity<ProveedorMaterial>()
                .HasKey(pm => new { pm.IdProveedor, pm.IdMaterial });

            modelBuilder.Entity<ProveedorMaterial>()
                .HasOne(pm => pm.Proveedor)
                .WithMany()
                .HasForeignKey(pm => pm.IdProveedor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProveedorMaterial>()
                .HasOne(pm => pm.Material)
                .WithMany()
                .HasForeignKey(pm => pm.IdMaterial)
                .OnDelete(DeleteBehavior.Restrict);

            // Restricciones de borrado y relaciones explícitas para Proyectos
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vinculamos explícitamente con la colección Proyectos del Usuario
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Proyectos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.EstadoProyecto)
                .WithMany()
                .HasForeignKey(p => p.EstadoId);

            // Relación de Proyectos con EstadoTaller (Nuevo)
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.EstadoTaller)
                .WithMany(e => e.Proyectos)
                .HasForeignKey(p => p.EstadoTallerId)
                .OnDelete(DeleteBehavior.Restrict);

            // CONFIGURACIÓN BLINDADA PARA HISTORIAL PROYECTO
            modelBuilder.Entity<HistorialProyecto>()
                .HasKey(h => h.HistorialId);

            modelBuilder.Entity<HistorialProyecto>()
                .HasOne(h => h.Proyecto)
                .WithMany(p => p.HistorialProyectos)
                .HasForeignKey(h => h.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialProyecto>()
                .HasOne(h => h.EstadoProyecto)
                .WithMany()
                .HasForeignKey(h => h.EstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}