using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients.TasasDeInteres;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using EFCore.BulkExtensions;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients.TasasDeInteres;

public class ServiceApiTasaDeInteresMoratorioPrestamoDia : ServiceUtils, IServiceApiTasaDeInteresMoratorioPrestamoDia
{

	public ServiceApiTasaDeInteresMoratorioPrestamoDia() : base(new CatalogosDbContext()) { }

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, string cartera)
	{
		ApiResultType result;
		jwtToken = token;
		dynamic response = GetDataFromApi(puntoDeConsumo, cartera);

		var entities = DataFix.ConvertTdimPorDiaToEntities(response);
		var entitiesToSave = new List<CtlInteresMoratorioDcp>();

		try
		{
			if (entities != null)
			{
				foreach (var item in entities)
				{
					var validation = ValidateEntity(item);
					if (validation)
						entitiesToSave.Add(item);
				}

				_catalogosDbContext.BulkInsert(entitiesToSave);

				result = ApiResultType.SUCCESS;
			}
			else
			{
				result = ApiResultType.NO_DATA;
			}
		}
		catch (Exception)
		{
			result = ApiResultType.ALREADY_SAVED;
		}

		return result;
	}

	private bool ValidateEntity(CtlInteresMoratorioDcp item)
	{
		var existRecord = _catalogosDbContext.CtlInteresMoratorioDcps.FirstOrDefault(i =>
						i.Fechains.Year == item.Fechains.Year &&
						i.Fechains.Month == item.Fechains.Month &&
						i.Fechains.Day == item.Fechains.Day);

		return existRecord == null;
	}
}
