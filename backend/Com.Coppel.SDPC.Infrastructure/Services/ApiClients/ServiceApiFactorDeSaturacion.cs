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
using Newtonsoft.Json.Linq;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServiceApiFactorDeSaturacion : ServiceUtils, IServiceApiFactorDeSaturacion
{
	public ServiceApiFactorDeSaturacion() : base(new CatalogosDbContext()) { }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6608:Prefer indexing instead of \"Enumerable\" methods on types implementing \"IList\"", Justification = "In this case we prefer Enumerable")]
	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera)
	{
		List<FactorDeSaturacionVM> data = [];
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo, cartera);

		if (response == null)
		{
			return ApiResultType.NO_DATA;
		}

		jsonSerializerSettings.SerializationBinder = Binders.FactorSaturacionRecord;
		string json = JsonConvert.SerializeObject(response, jsonSerializerSettings);

		if (json != null!)
		{
			var aux = JsonConvert.DeserializeObject<JObject>(json, jsonSerializerSettings);
			var fecha = (string)aux!.First!.Last!.Last!.First!;
			var x = aux.First!.Last!.First!;

			foreach (var item in x)
			{
				var plazo = 0;
				if (item.Path.Contains('.'))
					plazo = int.Parse(item.Path.Split(".").Last().Split("]['")[1].Replace("']", "").Split(' ')[1]);
				else
					plazo = int.Parse(item.Path.Split("]['")[1].Replace("']", "").Split(' ').Last());

				foreach (var detail in item.First!)
				{
					FactorDeSaturacionVM tmp = new()
					{
						factornormal = detail.SelectToken("numFactorNormal")!.Value<decimal>(),
						factorespecial = detail.SelectToken("numFactorEspecial")!.Value<decimal>(),
						factorinicial = detail.SelectToken("numFactorInicial")!.Value<decimal>(),
						factorminima = detail.SelectToken("numFactorMinima")!.Value<decimal>(),
						fechaArranque = fecha,
						plazo = plazo
					};
					data.Add(tmp);
				}
			}

			return ProcessData(cartera, data);
		}
		
		return ApiResultType.URI_NOT_FOUND;
	}

	private ApiResultType ProcessData(string cartera, List<FactorDeSaturacionVM> data)
	{
		ApiResultType result;
		try
		{
			var entities = ConvertrResusltIntoEnEntities(data, cartera);

			if (entities.Count < 1)
			{
				return ApiResultType.ALREADY_SAVED;
			}

			foreach (var item in entities)
			{
				if (item.FechaArranque.Year == 1900)
				{
					item.Estatus = (int)EstatusType.Finalizado;
				}
			}

			_catalogosDbContext.BulkInsert(entities);

			result = ApiResultType.SUCCESS;
		}
		catch (Microsoft.Data.SqlClient.SqlException)
		{
			result = ApiResultType.TABLE_NOT_FOUND;
		}
		catch (Exception)
		{
			result = ApiResultType.URI_NOT_FOUND;
		}

		return result;
	}

	private List<CtlFactoresSaturacionCartera> ConvertrResusltIntoEnEntities(List<FactorDeSaturacionVM> items, string cartera)
	{
		var entities = DataFix.ConvertFactoresDeSaturacionToEntities(items, cartera);
		var entitiesToSave = new List<CtlFactoresSaturacionCartera>();

		if (entities != null)
		{
			foreach (var item in entities)
			{
				var validation = ValidateEntity(item);
				if (validation)
					entitiesToSave.Add(item);
				else
					return [];
			}
		}

		return entitiesToSave;
	}

	private bool ValidateEntity(CtlFactoresSaturacionCartera item)
	{
		var existRecord = _catalogosDbContext.CtlFactoresSaturacionCarteras.FirstOrDefault(i =>
						i.Cartera.CompareTo(item.Cartera) == 0 &&
						i.FechaArranque.Year == item.FechaArranque.Year &&
						i.FechaArranque.Month == item.FechaArranque.Month &&
						i.FechaArranque.Day == item.FechaArranque.Day);

		return existRecord == null;
	}
}
