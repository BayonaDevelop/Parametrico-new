namespace Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;

public interface IServiceApiToken
{
	bool CanConnectToCatalogos(CancellationToken cancellationToken);

	bool UseIdc();

	string GetToken(CancellationToken cancellationToken);

	string GetTokenIDC();

	int GetMinutsBeforeTry(CancellationToken cancellationToken);
}
