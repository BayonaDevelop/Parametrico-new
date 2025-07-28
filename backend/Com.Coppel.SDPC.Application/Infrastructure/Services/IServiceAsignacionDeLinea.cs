namespace Com.Coppel.SDPC.Application.Infrastructure.Services;

public interface IServiceAsignacionDeLinea
{
	bool ProcessDaily(string token);

	bool ProcesssAfter20(string token);
}
