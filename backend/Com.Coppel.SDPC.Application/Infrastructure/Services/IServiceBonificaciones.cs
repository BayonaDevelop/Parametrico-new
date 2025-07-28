using Com.Coppel.SDPC.Application.Models.Persistence;

namespace Com.Coppel.SDPC.Application.Infrastructure.Services;

public interface IServiceBonificaciones
{
	bool ProcessParams(string token);

  bool ProcessCarterasAfter20(string token);
}
