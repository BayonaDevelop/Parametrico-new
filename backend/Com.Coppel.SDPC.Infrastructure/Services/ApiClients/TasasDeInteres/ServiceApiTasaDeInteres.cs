using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients.TasasDeInteres;
using Com.Coppel.SDPC.Application.Models.ApiModels;
using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using EFCore.BulkExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients.TasasDeInteres;

public class ServiceApiTasaDeInteres : ServiceUtils, IServiceApiTasaDeInteres
{
	public ServiceApiTasaDeInteres() : base(new CatalogosDbContext()) { }

	public ApiResultType GetDataForTdiByCartera(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera)
	{
		ApiResultType result = ApiResultType.URI_NOT_FOUND;
		List<CtlTasasInteresCp> entities = [];
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo, cartera);

		if (response != null)
		{
			jsonSerializerSettings.SerializationBinder = Binders.TasaInteresRecord;
			string json = JsonConvert.SerializeObject(response, jsonSerializerSettings);
			if (json != null)
			{
				List<TasaInteresVM> data = JsonConvert.DeserializeObject<List<TasaInteresVM>>(json!, jsonSerializerSettings!)!;
				entities = DataFix.ConvertTdiToEntity(data, cartera);
			}
		}

		try
		{
			List<CtlTasasInteresCp> entitiesToSave = [];
			entitiesToSave.AddRange(from CtlTasasInteresCp item in entities
															where ValidateEntity(item, cartera)
															select item);
			if (entitiesToSave.Count > 0)
			{
				_catalogosDbContext.CtlTasasInteresCps.AddRange(entities);
				_catalogosDbContext.SaveChanges();
				result = ApiResultType.SUCCESS;
			}
		}
		catch (Exception)
		{
			result = ApiResultType.ALREADY_SAVED;
		}

		return result;
	}
	 
	public ApiResultType GetDataForTdiByCarteraList(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera)
	{
		ApiResultType result;
		List<CtlTasasInteresCp> entitiesToSave = [];
		List<TasaInteresVM> data = [];
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo, cartera);

		if (response != null)
		{
			jsonSerializerSettings.SerializationBinder = Binders.TasaInteresRecord;
			string json = JsonConvert.SerializeObject(response, jsonSerializerSettings);
			data = JsonConvert.DeserializeObject<List<TasaInteresVM>>(json!, jsonSerializerSettings!)!;
		}

		try
		{
			var entities = DataFix.ConvertTdiToEntities(data!, cartera);
			foreach (var item in entities)
			{
				var validation = ValidateEntity(item, cartera);
				if (validation)
				{
					entitiesToSave.Add(item);
				}
			}

			_catalogosDbContext.BulkInsert(entitiesToSave);
			result = ApiResultType.SUCCESS;
		}
		catch (Exception)
		{
			result = ApiResultType.ALREADY_SAVED;
		}

		return result;
	}

	public ApiResultType GetDataForTdiByMuebles(string token, PuntoDeConsumoVM puntoDeConsumo)
	{
		ApiResultType result;
		List<TasaInteresMueblesVM> data = [];
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo);

		if (response != null)
		{
			var aux = response as JObject;
			var fecha = aux!.GetValue("fechaArranque")!.ToString();
			var items = aux!.GetValue("data");

			jsonSerializerSettings.SerializationBinder = Binders.TasaInteresMueblesRecord;
			string json = JsonConvert.SerializeObject(items, jsonSerializerSettings);
			if (json != null)
			{
				data = JsonConvert.DeserializeObject<List<TasaInteresMueblesVM>>(json, jsonSerializerSettings)!;
				foreach (var item in data)
				{
					item.fechaArranque = fecha;
				}
			}
		}

		try
		{
			var entities = DataFix.ConvertTdiMueblesToEntities(data);
			var entitiesToSave = new List<CtlTasasInteresCpc>();

			bool validation = false;
			foreach (var item in entities)
			{
				validation = ValidateEntityOfMuebles(item);
				if (validation)
				{
					entitiesToSave.Add(item);
				}
			}
			if (validation)
				_catalogosDbContext.BulkInsert<CtlTasasInteresCpc>(entitiesToSave);
			result = ApiResultType.SUCCESS;
		}
		catch (Exception)
		{
			result = ApiResultType.ALREADY_SAVED;
		}

		return result;
	}

	private bool ValidateEntity(CtlTasasInteresCp item, string cartera)
	{
		var existRecord = _catalogosDbContext.CtlTasasInteresCpcs.FirstOrDefault(i =>
			StringComparer.OrdinalIgnoreCase.Equals(i.Cartera, cartera) &&		
						
			i.FechaArranque.Year == item.FechaArranque.Year &&
			i.FechaArranque.Month == item.FechaArranque.Month &&
			i.FechaArranque.Day == item.FechaArranque.Day);

		if (existRecord == null)
		{
			return true;
		}

		return false;
	}

	private bool ValidateEntityOfMuebles(CtlTasasInteresCpc item)
	{
		var existRecord = _catalogosDbContext.CtlTasasInteresCpcs.FirstOrDefault(i =>
						i.FechaArranque.Year == item.FechaArranque.Year &&
						i.FechaArranque.Month == item.FechaArranque.Month &&
						i.FechaArranque.Day == item.FechaArranque.Day);

		if (existRecord == null)
		{
			return true;
		}

		return false;
	}
}
