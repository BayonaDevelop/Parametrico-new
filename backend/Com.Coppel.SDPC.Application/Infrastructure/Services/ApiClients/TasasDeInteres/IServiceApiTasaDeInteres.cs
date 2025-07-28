using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients.TasasDeInteres;

public interface IServiceApiTasaDeInteres
{
	ApiResultType GetDataForTdiByCartera(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera);

	ApiResultType GetDataForTdiByCarteraList(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera);

	ApiResultType GetDataForTdiByMuebles(string token, PuntoDeConsumoVM puntoDeConsumo);
}
