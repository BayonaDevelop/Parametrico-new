using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services.TasasDeInteres
{
	public interface IServiceTasasDeInteres
	{
		bool ProcessParams(PuntoDeConsumoVM puntoDeConsumo);
	}
}
