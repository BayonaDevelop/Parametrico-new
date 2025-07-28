using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services.TasasDeInteres;

public interface IServiceTasasDeInteresMoratorioPrestamoDia
{
	public bool ProcessParameters(PuntoDeConsumoVM puntoDeConsumo);
}
