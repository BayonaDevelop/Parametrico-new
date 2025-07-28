using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Core.Emision20;
using Microsoft.EntityFrameworkCore;
using System;

namespace Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;

public partial class Emision20DbContext : DbContext
{
	public Emision20DbContext() {}

	public Emision20DbContext(DbContextOptions<CatalogosDbContext> options) : base(options) {}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer(Utils.GetConnectionStrings().Find(i => StringComparer.OrdinalIgnoreCase.Equals(i.Key, Enum.GetName(typeof(DatabaseType), DatabaseType.Emision20)!)).Value);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3251:Implementations should be provided for \"partial\" methods", Justification = "DotNet definition")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "NetCore definition")]
	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3251:Implementations should be provided for \"partial\" methods", Justification = "NetCore definition")]
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CatBonificacionesPrestamo>(entity =>
		{
			entity.HasNoKey();

			entity.ToTable("cat_bonificacionesPrestamos");

			entity.Property(e => e.FecCorte)
					.HasColumnType("date")
					.HasColumnName("fec_corte");

			entity.Property(e => e.FecMovimiento)
					.HasColumnType("smalldatetime")
					.HasColumnName("fec_movimiento")
					.HasDefaultValueSql("(CONVERT([smalldatetime],getdate(),(0)))");

			entity.Property(e => e.NumDiasTranscurridos).HasColumnName("num_diasTranscurridos");

			entity.Property(e => e.NumPlazo)
					.HasColumnName("num_plazo")
					.HasDefaultValueSql("((0))");

			entity.Property(e => e.PrcBonificacion)
					.HasColumnType("numeric(10, 4)")
					.HasColumnName("prc_bonificacion");

			entity.Property(e => e.PrcBonificacionNueva)
					.HasColumnType("numeric(10, 4)")
					.HasColumnName("prc_bonificacionNueva");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	public virtual DbSet<CatBonificacionesPrestamo> CatBonificacionesPrestamos { get; set; } = null!;
}
