using Com.Coppel.SDPC.Application.Infrastructure.Services;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Com.Coppel.SDPC.Infrastructure.Services;

public class ServicePuntosDeConsumo(ILogger<ServicePuntosDeConsumo> log) : ServiceUtils(new CatalogosDbContext()), IServicePuntosDeConsumo 
{
	public IEnumerable<PuntoDeConsumoVM> GetAllAsync()
	{
		log.LogInformation("A huevo");
		return [.. _catalogosDbContext.CtlPuntosdeconsumos
			.AsNoTracking()
			.Select(i => new PuntoDeConsumoVM
			{
				IdFuncionalidad = i.IdFuncionalidad,
				NomFuncionalidad = i.NomFuncionalidad!,
				NomTbDestino = i.NomTbDestino!,
				RutaServicio = i.RutaServicio!,
				AllowAfter20 = i.AllowAfter20!.Value,
				Flag = i.Flag!.Value
			})];
	}
}
