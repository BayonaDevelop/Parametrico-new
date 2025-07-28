using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.ApiModels;
using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using EFCore.BulkExtensions;
using Newtonsoft.Json;
using System.Globalization;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServiceApiDecrementoLineaCredito : ApiClient, IServiceApiDecrementoLineaCredito
{
	private readonly CatalogosDbContext _catalogosDbContext;

	public ServiceApiDecrementoLineaCredito()
	{
		_catalogosDbContext = new();
	}

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo)
	{
		ApiResultType result;
		List<CtlDecrementosLineadeCredito> entities = [];
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
			response = FetchStringJsonFromGetAsync($"{puntoDeConsumo.RutaServicio}", null!, new CancellationToken())
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
		}

		if (response != null)
		{
			string json = JsonConvert.SerializeObject(response);
			if (json != null)
			{
				jsonSerializerSettings.SerializationBinder = Binders.DecrementoLineaRecord;
				List<DecrementoLineaCreditoVM> data = JsonConvert.DeserializeObject<List<DecrementoLineaCreditoVM>>(json!, jsonSerializerSettings)!;
				entities = ConvertrResusltIntoEnEntities(data!);
			}
		}

		try
		{
			if (Validate(entities[0]))
			{
				if (entities.Any(i => i.FechaArranque.Year == 1900))
				{
					foreach (var item in entities)
					{
						item.Estatus = (int)EstatusType.Finalizado;
					}
				}

				_catalogosDbContext.BulkInsert<CtlDecrementosLineadeCredito>(entities);
				result = ApiResultType.SUCCESS;
			}
			else
			{
				result = ApiResultType.ALREADY_SAVED;
			}
		}
		catch (Exception)
		{
			result = ApiResultType.URI_NOT_FOUND;
		}

		return result;
	}

	private static List<CtlDecrementosLineadeCredito> ConvertrResusltIntoEnEntities(List<DecrementoLineaCreditoVM> result)
	{
		try
		{
			List<CtlDecrementosLineadeCredito> decrementosLineadeCreditos = [];
			foreach (DecrementoLineaCreditoVM item in result)
			{
				CtlDecrementosLineadeCredito regDecrementoLinea = new()
				{
					FechaAlta = DateTime.Now,
					FechaActualizacion = null,
					Estatus = 1,
					NumTipoLinea = item.numtipolc,
					NomTipoLinea = item.nomtipolinea,
					NumTopeLineaMinima = item.numtopelineaminima,
					NomPuntualidad = item.nompuntualidad,
					NumEficienciaHis = item.numeficienciahistorica,
					NumDecremento = item.numdecremento,
					FechaArranque = DateTime.ParseExact(item.fechaarranque, "dd-MM-yyyy", CultureInfo.InvariantCulture)
				};
				decrementosLineadeCreditos.Add(regDecrementoLinea);
			}
			return decrementosLineadeCreditos;
		}
		catch (Exception)
		{
			return [];
		}
	}

	private bool Validate(CtlDecrementosLineadeCredito entity)
	{
		bool result = false;

		var existRecord = _catalogosDbContext.CtlDecrementosLineadeCreditos.FirstOrDefault(i =>
						i.FechaArranque.Year == entity.FechaArranque.Year &&
						i.FechaArranque.Month == entity.FechaArranque.Month &&
						i.FechaArranque.Day == entity.FechaArranque.Day);

		if (existRecord == null)
		{
			result = true;
		}

		return result;
	}
}
