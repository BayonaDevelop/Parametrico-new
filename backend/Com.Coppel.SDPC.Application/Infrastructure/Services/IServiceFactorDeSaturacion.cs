using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services;

public interface IServiceFactorDeSaturacion
{
	bool ProcessParams(string token);

	bool ProcessParamsAfter20(string token);

	List<CtlCarterasFactoreSaturacion> GetListOfCarteras();
}
