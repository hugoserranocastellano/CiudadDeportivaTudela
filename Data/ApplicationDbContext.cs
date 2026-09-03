using CiudadDeportivaTudela.Models;
using Microsoft.EntityFrameworkCore;

namespace CiudadDeportivaTudela.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Socio> Socios => Set<Socio>();

    public DbSet<Articulo> Articulos => Set<Articulo>();

    public DbSet<CategoriaGasto> CategoriasGasto => Set<CategoriaGasto>();

    public DbSet<GastoSociedad> GastosSociedad => Set<GastoSociedad>();

    public DbSet<Mesa> Mesas => Set<Mesa>();

    public DbSet<Reserva> Reservas => Set<Reserva>();

    public DbSet<TipoReserva> TiposReserva => Set<TipoReserva>();

    public DbSet<ReservaMesa> ReservaMesas => Set<ReservaMesa>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<TicketLinea> TicketLineas => Set<TicketLinea>();

    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();

    public DbSet<EventoInscripcion> EventoInscripciones => Set<EventoInscripcion>();

    public DbSet<Sugerencia> Sugerencias => Set<Sugerencia>();

    public DbSet<CategoriaSocio> CategoriasSocios => Set<CategoriaSocio>();

    public DbSet<Evento> Eventos => Set<Evento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Socio>(entity =>
        {
            entity.ToTable("socios");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Dni).HasColumnName("dni");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Apellidos).HasColumnName("apellidos");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.UrlFoto).HasColumnName("url_foto");
            entity.Property(e => e.PinHash).HasColumnName("pin_hash");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria");
            entity.Property(e => e.NumeroCuenta).HasColumnName("numero_cuenta");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.FechaAlta).HasColumnName("fecha_alta");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.UltimoAcceso).HasColumnName("UltimoAcceso");
            entity.Property(e => e.ValidezPin).HasColumnName("ValidezPin");

            entity.HasOne(e => e.CategoriaSocio)
                .WithMany()
                .HasForeignKey(e => e.CategoriaId);
        });

        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.ToTable("articulos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.UrlFoto).HasColumnName("url_foto");
            entity.Property(e => e.PrecioUnidad).HasColumnName("precio_unidad").HasColumnType("numeric(10,2)");
            entity.Property(e => e.Proveedor).HasColumnName("proveedor");
            entity.Property(e => e.StockInicial).HasColumnName("stock_inicial");
            entity.Property(e => e.StockActual).HasColumnName("stock_actual");
            entity.Property(e => e.StockMinimo).HasColumnName("stock_minimo");
            entity.Property(e => e.Activo).HasColumnName("activo");
        });

        modelBuilder.Entity<CategoriaGasto>(entity =>
        {
            entity.ToTable("categorias_gasto");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.TipoCategoria).HasColumnName("tipo_categoria").IsRequired();
        });

        modelBuilder.Entity<GastoSociedad>(entity =>
        {
            entity.ToTable("gastos_sociedad");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id");
            entity.Property(e => e.Fecha).HasColumnName("fecha").HasColumnType("date");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad").HasColumnType("numeric(10,2)");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Gastos)
                .HasForeignKey(e => e.CategoriaId);
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.ToTable("mesas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.Estancia).HasColumnName("estancia");
            entity.Property(e => e.Capacidad).HasColumnName("capacidad");
            entity.Property(e => e.Activa).HasColumnName("activa");
            entity.Property(e => e.UrlFotoMesa).HasColumnName("url_foto_mesa");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("reservas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha).HasColumnName("fecha").HasColumnType("date");
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.Comensales).HasColumnName("comensales");
            entity.Property(e => e.Limpieza).HasColumnName("limpieza");
            entity.Property(e => e.PagadaLimpieza).HasColumnName("PagadaLimpieza");
            entity.Property(e => e.TipoReservaId).HasColumnName("tipo_reserva");
            entity.Property(e => e.Estado).HasColumnName("estado");
            // El default de la columna es now(): así EF omite created_at al insertar null.
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Socio)
                .WithMany()
                .HasForeignKey(e => e.SocioId);

            entity.HasOne(e => e.TipoReserva)
                .WithMany(t => t.Reservas)
                .HasForeignKey(e => e.TipoReservaId);
        });

        modelBuilder.Entity<TipoReserva>(entity =>
        {
            entity.ToTable("tipo_reserva");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id_tipo_reserva");
            entity.Property(e => e.Descripcion).HasColumnName("TipoReserva").IsRequired();
            entity.Property(e => e.HoraInicio).HasColumnName("HoraInicio");
            entity.Property(e => e.HoraFin).HasColumnName("HoraFin");
            entity.Property(e => e.Orden).HasColumnName("Orden");
        });

        modelBuilder.Entity<ReservaMesa>(entity =>
        {
            entity.ToTable("reserva_mesas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReservaId).HasColumnName("reserva_id");
            entity.Property(e => e.MesaId).HasColumnName("mesa_id");

            entity.HasOne(e => e.Reserva)
                .WithMany(r => r.ReservaMesas)
                .HasForeignKey(e => e.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Mesa)
                .WithMany(m => m.ReservaMesas)
                .HasForeignKey(e => e.MesaId);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha).HasColumnName("fecha").HasDefaultValueSql("now()");
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.ImporteTotal).HasColumnName("importe_total").HasColumnType("numeric(10,2)");
            entity.Property(e => e.Revisado).HasColumnName("revisado");
            entity.Property(e => e.UrlFotoTicket).HasColumnName("url_foto_ticket");
            entity.Property(e => e.Estado).HasColumnName("estado").HasDefaultValue("abierto");
            entity.Property(e => e.FormaPago).HasColumnName("forma_pago");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Socio)
                .WithMany()
                .HasForeignKey(e => e.SocioId);
        });

        modelBuilder.Entity<TicketLinea>(entity =>
        {
            entity.ToTable("ticket_lineas");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.ArticuloId).HasColumnName("articulo_id");
            entity.Property(e => e.PrecioUnidad).HasColumnName("precio_unidad").HasColumnType("numeric(10,2)");
            entity.Property(e => e.Unidades).HasColumnName("unidades");
            entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("numeric(10,2)");

            entity.HasOne(e => e.Ticket)
                .WithMany(t => t.Lineas)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Articulo)
                .WithMany(a => a.Lineas)
                .HasForeignKey(e => e.ArticuloId);
        });

        modelBuilder.Entity<MovimientoStock>(entity =>
        {
            entity.ToTable("movimientos_stock");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArticuloId).HasColumnName("articulo_id");
            entity.Property(e => e.TipoMovimiento).HasColumnName("tipo_movimiento");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Articulo)
                .WithMany(a => a.Movimientos)
                .HasForeignKey(e => e.ArticuloId);

            entity.HasOne(e => e.Ticket)
                .WithMany()
                .HasForeignKey(e => e.TicketId);
        });

        modelBuilder.Entity<EventoInscripcion>(entity =>
        {
            entity.ToTable("evento_inscripciones");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventoId).HasColumnName("evento_id");
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.Acude).HasColumnName("acude");
            entity.Property(e => e.ListaEspera).HasColumnName("lista_espera");
            entity.Property(e => e.FechaInscripcion).HasColumnName("fecha_inscripcion").HasDefaultValueSql("now()");

            entity.HasOne(e => e.Socio)
                .WithMany()
                .HasForeignKey(e => e.SocioId);
        });

        modelBuilder.Entity<Sugerencia>(entity =>
        {
            entity.ToTable("sugerencias");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Titulo).HasColumnName("TituloSugerencia");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.SocioId).HasColumnName("socio_id");
            entity.Property(e => e.Fecha).HasColumnName("fecha").HasDefaultValueSql("now()");
            entity.Property(e => e.Respuesta).HasColumnName("respuesta");
            entity.Property(e => e.Visible).HasColumnName("visible");
            entity.Property(e => e.FechaRespuesta).HasColumnName("fecha_respuesta");

            entity.HasOne(e => e.Socio)
                .WithMany()
                .HasForeignKey(e => e.SocioId);
        });

        modelBuilder.Entity<CategoriaSocio>(entity =>
        {
            entity.ToTable("CategoriasSocios");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id_Categoria");
            entity.Property(e => e.Junta).HasColumnName("JUNTA").IsRequired();
            entity.Property(e => e.TipoSocio).HasColumnName("TIPO_SOCIO");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.ToTable("eventos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired();
            entity.Property(e => e.Fecha).HasColumnName("fecha").HasColumnType("date");
            entity.Property(e => e.Caracteristicas).HasColumnName("caracteristicas");
            entity.Property(e => e.Obligatorio).HasColumnName("obligatorio");
            entity.Property(e => e.PlazasMaximas).HasColumnName("plazas_maximas");
        });

    }
}
