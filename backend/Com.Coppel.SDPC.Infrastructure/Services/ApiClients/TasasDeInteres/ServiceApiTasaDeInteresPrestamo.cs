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

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients.TasasDeInteres;

public class ServiceApiTasaDeInteresPrestamo : ServiceUtils, IServiceApiTasaDeInteresPrestamo
{
	public ServiceApiTasaDeInteresPrestamo() : base(new CatalogosDbContext()) { }
		
	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera)
	{
		ApiResultType result;
		List<TasaInteresPrestamoVM> itemsList = [];
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo, cartera);

		if (response != null)
		{
			jsonSerializerSettings.SerializationBinder = Binders.PrestamoRecord;
			string json = JsonConvert.SerializeObject(response, jsonSerializerSettings);
			if (json != null)
			{
				var data = JsonConvert.DeserializeObject<PrestamosBaseVM>(json, jsonSerializerSettings);

				foreach (var item in data!.data)
				{
					item.fechaArranque = data.fechaarranque;
				}
				itemsList = data.data;
			}
		}

		try
		{
			List<CtlTasaInteresCpprestamo> entities = DataFix.ConvertTdiPrestamosToEntities(itemsList, cartera);
			result = ProcessData(entities);	
		}
		catch (Exception)
		{
			result = ApiResultType.ALREADY_SAVED;
		}

		return result;
	}

	private ApiResultType ProcessData(List<CtlTasaInteresCpprestamo> entities)
	{
		List<CtlTasaInteresCpprestamo> entitiesToSave = [];

		if (entities != null)
		{
			foreach (var item in entities)
			{
				var validation = ValidateEntity(item);
				if (validation)
				{
					entitiesToSave.Add(item);
				}
			}

			_catalogosDbContext.BulkInsert(entitiesToSave);
			return ApiResultType.SUCCESS;
		}
		else
		{
			return ApiResultType.NO_DATA;
		}
	}

	private bool ValidateEntity(CtlTasaInteresCpprestamo item)
	{
		var existRecord = _catalogosDbContext.CtlTasaInteresCpprestamos.FirstOrDefault(i =>
						i.Puntualidad.CompareTo(item.Puntualidad) == 0 &&
						i.FechaArranque.Year == item.FechaArranque.Year &&
						i.FechaArranque.Month == item.FechaArranque.Month &&
						i.FechaArranque.Day == item.FechaArranque.Day);

		return existRecord == null;
	}
}
