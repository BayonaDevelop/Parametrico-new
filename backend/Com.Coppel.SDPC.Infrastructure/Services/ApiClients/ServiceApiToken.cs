using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.Services;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServiceApiToken : ApiClient, IServiceApiToken
{
	private readonly CatalogosDbContext _catalogosDbContext;

	public ServiceApiToken()
	{
		_catalogosDbContext = new();
	}

	public bool CanConnectToCatalogos(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return _catalogosDbContext.Database.CanConnect();
	}

	public bool UseIdc()
	{
		try
		{
			CtlParametrosautenticacion parameter = _catalogosDbContext.CtlParametrosautenticacions
			.AsNoTracking()
			.FirstOrDefault(i => i.Idc == 2 && i.NombreParametro!.CompareTo("flagIDC") == 0)!;

			return parameter != null && parameter.ValorParametro!.CompareTo("1") == 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public string GetToken(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		try
		{
			var parameters = _catalogosDbContext.CtlParametrosautenticacions
			.Where(i => i.Idc == 0)
			.AsNoTracking()
			.ToList();

			var tokenRequest = new TokenRequestVM
			{
				AppUrl = parameters.First(i => i.NombreParametro!.CompareTo("appUrl") == 0).ValorParametro!,
				AppId = parameters.First(i => i.NombreParametro!.CompareTo("appId") == 0).ValorParametro!,
				AppKey = parameters.First(i => i.NombreParametro!.CompareTo("appKey") == 0).ValorParametro!
			};

			dynamic requestBody = new
			{
				appId = tokenRequest.AppId,
				appKey = tokenRequest.AppKey
			};

			string result = FetchStringJsonFromPostAsync(tokenRequest.AppUrl, requestBody, new CancellationToken());
			return result;
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public string GetTokenIDC()
	{
		try
		{
			var parameters = _catalogosDbContext.CtlParametrosautenticacions
			.Where(i => i.Idc == 1)
			.AsNoTracking()
			.ToList();

			string url = parameters.Find(i => i.Tipo!.CompareTo("URL") == 0)!.ValorParametro!;
			List<KeyValuePair<string, string>> headers = [];
			List<KeyValuePair<string, string>> formVaslues = [];

			foreach (CtlParametrosautenticacion parameter in parameters.Where(i => i.Tipo!.CompareTo("HEADER") == 0))
			{
				if (parameter.NombreParametro!.CompareTo("X-Coppel-Date-Request") == 0)
				{
					DateTime requestDate = DateTime.Now;
					string formattedDate = requestDate.ToString("yyyy-MM-dd'T'HH:mm:ssZ"); /// 2025-02-27T05:06:07Z
					headers.Add(new KeyValuePair<string, string>(parameter.NombreParametro!, formattedDate));
				}
				else if (parameter.NombreParametro!.CompareTo("X-Coppel-TransactionId") == 0)
				{
					headers.Add(new KeyValuePair<string, string>(parameter.NombreParametro!, Guid.NewGuid().ToString()));
				}
				else
				{
					headers.Add(new KeyValuePair<string, string>(parameter.NombreParametro!, parameter.ValorParametro!));
				}
			}

			foreach (CtlParametrosautenticacion parameter in parameters.Where(i => i.Tipo!.CompareTo("FORM") == 0))
			{
				formVaslues.Add(new KeyValuePair<string, string>(parameter.NombreParametro!, parameter.ValorParametro!));
			}

			return FetchTokenJsonFromPostAsync(url, headers, formVaslues, new CancellationToken()).Result;
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public int GetMinutsBeforeTry(CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		CtlParametrosautenticacion parameter = _catalogosDbContext.CtlParametrosautenticacions.FirstOrDefault(i => i.NombreParametro!.CompareTo("MinutosEnEsperaPorReintento") == 0)!;

		if (parameter == null)
		{
			return 0;
		}
		else
		{
			return int.Parse(parameter.ValorParametro!);
		}
	}
}
