using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services.SalariosMinimos;

public interface IServiceSalariosMinimosCp
{
	Task<bool> ProcessParametersAsync(PuntoDeConsumoVM puntoDeConsumo, CancellationToken cancellationToken);
}
