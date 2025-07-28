using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients.TasasDeInteres;
using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;
using Com.Coppel.SDPC.Application.Models.ApiModels;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Newtonsoft.Json;
using System.Diagnostics;
using Com.Coppel.SDPC.Infrastructure.Commons;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients.TasasDeInteres;

public class ServiceApiTasaDeInteresMoratorio : ServiceUtils, IServiceApiTasaDeInteresMoratorio
{
	public ServiceApiTasaDeInteresMoratorio() : base(new CatalogosDbContext()) { }

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo)
	{
		TasaInteresMoratorioVM result = new();
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo);

		if (response == null)
		{
			return ApiResultType.NO_DATA;
		}

		try
		{
			if (response == ApiResultType.URI_NOT_FOUND)
			{
				return ApiResultType.URI_NOT_FOUND;
			}
		}
		catch (Exception) { Debug.WriteLine("No aplica esta validación"); }

		jsonSerializerSettings.SerializationBinder = Binders.TasaInteresMoratorioRecord;
		string json = JsonConvert.SerializeObject(response, jsonSerializerSettings);
		if (json != null)
		{
			result = JsonConvert.DeserializeObject<TasaInteresMoratorioVM>(json, jsonSerializerSettings)!;
		}


		try
		{
			var entity = DataFix.ConvertTdimToEntity(result);
			if (ValidateEntity(entity))
			{
				if (entity.FechaArranque.Year == 1900)
				{
					entity.Estatus = (int)EstatusType.Finalizado;
					entity.EstatusCl = (int)EstatusType.Finalizado;
				}
				_catalogosDbContext.CtlInteresMoratorioporCarteras.Add(entity);
				_catalogosDbContext.SaveChanges();
				return ApiResultType.SUCCESS;
			}
			else
			{
				return ApiResultType.ALREADY_SAVED;
			}
		}
		catch (Microsoft.Data.SqlClient.SqlException)
		{
			return ApiResultType.TABLE_NOT_FOUND;
		}
		catch (Exception)
		{
			return ApiResultType.URI_NOT_FOUND;
		}
	}

	private bool ValidateEntity(CtlInteresMoratorioporCartera item)
	{
		var existRecord = _catalogosDbContext.CtlInteresMoratorioporCarteras.FirstOrDefault(i =>
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
