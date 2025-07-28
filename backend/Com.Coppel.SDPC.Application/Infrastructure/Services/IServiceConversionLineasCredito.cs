using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services;

public interface IServiceConversionLineasCredito
{
	bool ProcessParams(PuntoDeConsumoVM puntoDeConsumo);

	bool ProcessCarterasAfter20(PuntoDeConsumoVM puntoDeConsumo);
}
