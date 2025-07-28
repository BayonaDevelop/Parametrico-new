using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;

public interface IServiceApiBonificaciones
{
	ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, CtlCatalogoPlazo plazo);
}
