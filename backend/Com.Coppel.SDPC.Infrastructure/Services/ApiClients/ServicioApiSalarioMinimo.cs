using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.ApiModels;
using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Newtonsoft.Json;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServicioApiSalarioMinimo(CatalogosDbContext catalogosDbContext) : ApiClient, IServiceApiSalarioMinimo
{
	private readonly CatalogosDbContext _catalogosDbContext = catalogosDbContext;

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ApiResultType result = ApiResultType.URI_NOT_FOUND;

		try
		{
			jwtToken = token;
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
				response = FetchStringJsonFromGetAsync(puntoDeConsumo.RutaServicio, string.Empty, cancellationToken)
					.ConfigureAwait(false)
					.GetAwaiter()
					.GetResult();
			}

			if (response != null)
			{
				jsonSerializerSettings.SerializationBinder = Binders.SalarioMinimoRecord;
				string json = JsonConvert.SerializeObject(response, jsonSerializerSettings);
				if (json != null)
				{
					var data = JsonConvert.DeserializeObject<SalarioMinimoVM>(json, jsonSerializerSettings)!;
					result = puntoDeConsumo.NomTbDestino.CompareTo("ctl_SalarioMinimoCP") == 0 ?
						ProcessCp(data) :
						ProcessLcAsync(data);
				}
			}
			else
			{
				result = ApiResultType.NO_DATA;
			}

			return result;
		}
		catch (Exception)
		{
			result = ApiResultType.URI_NOT_FOUND;
		}

		return result;
	}

	private ApiResultType ProcessCp(SalarioMinimoVM data)
	{
		ApiResultType result;

		try
		{
			CtlSalarioMinimoCp entity = DataFix.ConvertSalarioMinimoCpToEntity(data);
			bool isValidEntity = ValidarSalarimoMinimoCp(entity);

			if (isValidEntity)
			{
				if (entity.FechaArranque.Year == 1900)
				{
					entity.Estatus = (int)EstatusType.Finalizado;
				}
				_catalogosDbContext.CtlSalarioMinimoCps.Add(entity);
				_catalogosDbContext.SaveChanges();
				result = ApiResultType.SUCCESS;
			}
			else
			{
				result = ApiResultType.ALREADY_SAVED;
			}
		}
		catch (Exception)
		{
			result = ApiResultType.TABLE_NOT_FOUND;
		}

		return result;
	}

	private ApiResultType ProcessLcAsync(SalarioMinimoVM data)
	{
		ApiResultType result;

		try
		{
			CtlSalarioMinimoLc entity = DataFix.ConvertSalarioMinimoLcToEntity(data);
			bool isValidEntity = ValidarSalarimoMinimoLc(entity);

			if (isValidEntity)
			{
				if (entity.FechaArranque.Year == 1900)
				{
					entity.Estatus = (int)EstatusType.Finalizado;
				}
				_catalogosDbContext.CtlSalarioMinimoLcs.Add(entity);
				_catalogosDbContext.SaveChanges();
				result = ApiResultType.SUCCESS;
			}
			else
			{
				result = ApiResultType.ALREADY_SAVED;
			}
		}
		catch (Exception)
		{
			result = ApiResultType.TABLE_NOT_FOUND;
		}

		return result;
	}

	private bool ValidarSalarimoMinimoCp(CtlSalarioMinimoCp entity)
	{
		var existRecord = _catalogosDbContext.CtlSalarioMinimoCps.FirstOrDefault(i =>
						i.FechaArranque.Year == entity.FechaArranque.Year &&
						i.FechaArranque.Month == entity.FechaArranque.Month &&
						i.FechaArranque.Day == entity.FechaArranque.Day);

		if (existRecord == null)
		{
			return true;
		}

		return false;
	}

	private bool ValidarSalarimoMinimoLc(CtlSalarioMinimoLc entity)
	{
		var existRecord = _catalogosDbContext.CtlSalarioMinimoLcs.FirstOrDefault(i =>
						i.FechaArranque.Year == entity.FechaArranque.Year &&
						i.FechaArranque.Month == entity.FechaArranque.Month &&
						i.FechaArranque.Day == entity.FechaArranque.Day);

		if (existRecord == null)
		{
			return true;
		}

		return false;
	}
}
