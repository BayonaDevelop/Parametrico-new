using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Newtonsoft.Json.Linq;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServiceApiAsignacionDeLineaGrupo : ServiceUtils, IServiceApiAsignacionDeLineaGrupo
{
	public ServiceApiAsignacionDeLineaGrupo() :base(new CatalogosDbContext()) { }

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo)
	{
		jwtToken = token;

		try
		{
			dynamic response = GetDataFromApi(puntoDeConsumo);

			if (response == null)
			{
				return ApiResultType.NO_DATA;
			}

			JObject json = (JObject)response;
			JEnumerable<JToken> data = json.SelectToken("data")!.Children();
			JArray gruposLineaJson = [];
			DateTime fechaArranque = DateTime.Today;

			foreach (var item in data)
			{
				if (item.SelectToken("fechaArranque") != null)
				{
					fechaArranque = GetDate(item!.SelectToken("fechaArranque")!.Value<string>()!);
				}
				else
				{
					gruposLineaJson.Add(item);
				}
			}
			List<CtlAsignacionDeGrupo> entities = [];

			foreach (var item in gruposLineaJson)
			{
				entities.Add(new()
				{
					FechaAlta = DateTime.Today,
					Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
					Idgrupo = short.Parse(item.SelectToken("idgrupo")!.ToString()),
					Grupo = item.SelectToken("grupo")!.ToString(),
					Terminacioncliente = item.SelectToken("terminacionCliente")!.ToString(),
					Numproceso = short.Parse(item.SelectToken("numProceso")!.ToString()),
					FechaArranque = fechaArranque
				});
			}

			if (Validate(fechaArranque))
			{
				_catalogosDbContext.CtlAsignacionDeGrupos.AddRange(entities);
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

	private static DateTime GetDate(string jsonValue)
	{
		string[] dateParts = jsonValue.Split("-");

		return new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]), 0, 0, 0, DateTimeKind.Local);
	}
	private bool Validate(DateTime fechaArranque)
	{
		var existRecord = _catalogosDbContext.CtlAsignacionDeGrupos.FirstOrDefault(i =>
						i.FechaArranque.Year == fechaArranque.Year &&
						i.FechaArranque.Month == fechaArranque.Month &&
						i.FechaArranque.Day == fechaArranque.Day);

		if (existRecord == null)
		{
			return true;
		}

		return false;
	}
}
