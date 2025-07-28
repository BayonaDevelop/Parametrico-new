using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients.TasasDeInteres;

public interface IServiceApiTasaDeInteresMoratorioPrestamoDia
{
	ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera);
}
