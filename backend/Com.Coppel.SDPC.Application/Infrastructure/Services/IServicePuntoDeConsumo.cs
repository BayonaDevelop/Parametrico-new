using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services;

public interface IServicePuntosDeConsumo
{
	IEnumerable<PuntoDeConsumoVM> GetAllAsync();
}
