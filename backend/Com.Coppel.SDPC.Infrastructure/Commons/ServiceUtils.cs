using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;

namespace Com.Coppel.SDPC.Infrastructure.Commons;

public class ServiceUtils(CatalogosDbContext catalogosDbContext) : ApiClient
{
	protected readonly CatalogosDbContext _catalogosDbContext = catalogosDbContext;

	protected dynamic GetDataFromApi(PuntoDeConsumoVM puntoDeConsumo, string cartera = null!)
	{
		dynamic response;

		if (Utils.IsInTesting())
		{
			string basePath = $"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!.Replace("file:\\", "")!}\\Assets\\";
			string dataPath = $"{basePath}Data\\Catalogos\\{puntoDeConsumo.IdFuncionalidad}\\1-Api.json";

			string jsonData = File.ReadAllText(dataPath);

			response = FetchStringJsonFromGetAsync(puntoDeConsumo.RutaServicio, jsonData, new CancellationToken())
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
		}
		else
		{
			string carteraString = cartera ?? string.Empty;
			string uri = $"{puntoDeConsumo.RutaServicio}{(cartera == null? "" : carteraString)}";
			response = FetchStringJsonFromGetAsync(uri, null!, new CancellationToken())
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
		}

		return response;
	}
}
