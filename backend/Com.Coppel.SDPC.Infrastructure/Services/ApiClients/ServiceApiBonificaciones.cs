using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.Bonificaciones;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServiceApiBonificaciones : ServiceUtils, IServiceApiBonificaciones
{
	public ServiceApiBonificaciones() : base(new CatalogosDbContext()) { }

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo, CtlCatalogoPlazo plazo)
	{
		this.jwtToken = token;

		try
		{
			puntoDeConsumo.RutaServicio = $"{puntoDeConsumo.RutaServicio}{plazo.IduCuenta},{plazo.Plazo},{plazo.TipoTransaccion}";
			dynamic response = GetDataFromApi(puntoDeConsumo);

			if (response == null)
			{
				return ApiResultType.NO_DATA;
			}

			BonificacionDetailVM items = MapData(response) ?? null!;
			var fechaArranque = items.fechaarranque;
			var months = items.plazos.DistinctBy(i => i.Plazo).ToList();
			var plazosList = _catalogosDbContext.CtlCatalogoPlazos.ToList();

			return ProccessMonths(months, items, plazosList, plazo, fechaArranque);
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

	private ApiResultType ProccessMonths(List<BonificacionPlazoVM> months, BonificacionDetailVM items, List<CtlCatalogoPlazo> plazosList, CtlCatalogoPlazo plazo, DateTime fechaArranque)
	{
		foreach (var (month, plazoTmp, validation) in from month in months!
																									let plazoTmp = plazosList.FirstOrDefault(i => i.IduCuenta == plazo.IduCuenta && i.IduCartera == plazo.IduCartera && i.TipoTransaccion.CompareTo(plazo.TipoTransaccion) == 0 && i.Plazo == month.Plazo)
																									let validation = ValidateEntity(fechaArranque, plazoTmp!)
																									select (month, plazoTmp, validation))
		{
			if (validation)
			{
				List<CtlBonificacionCartera> entities = items.plazos
					 .Where(i => i.Plazo == month.Plazo)
					 .Select(i => new CtlBonificacionCartera
					 {
						 FechaAlta = DateTime.Now,
						 FechaArranque = fechaArranque,
						 Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
						 EstatusCl = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
						 IdPlazo = plazoTmp!.IdPlazo,
						 NumDiastranscurridos = (short)i.diastranscurridos,
						 NumPorcentajebonificacion = i.porcentajebonificacion
					 })
					 .ToList();

				_catalogosDbContext.CtlBonificacionCarteras.AddRange(entities);
				_catalogosDbContext.SaveChanges();
			}
			else
			{
				return ApiResultType.ALREADY_SAVED;
			}
		}

		return ApiResultType.SUCCESS;
	}


	[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6608:Prefer indexing instead of \"Enumerable\" methods on types implementing \"IList\"", Justification = "We wiññ use IList instead of counting")]
	private static BonificacionDetailVM MapData(dynamic response)
	{
		BonificacionDetailVM result = new();
		try
		{
			var json = (JToken)response;

			JEnumerable<JToken> data = json!.First!.First!.First!.Children();

			foreach (JToken item in data)
			{
				bool skipItem = false;
				foreach (var pair in item)
				{
					skipItem = pair.Path.Contains("fechaarranque");
					if (skipItem)
					{
						break;
					}
				}

				if (!skipItem)
				{
					var termText = item.Path
							.Split('[')
							.Last()
							.Replace("]", "")
							.Replace("'", "")
							.Split(' ')
							.Last();
					var textDetails = JsonConvert.SerializeObject(item.Values());
					var tmpData = JsonConvert.DeserializeObject<List<BonificacionPlazoVM>>(textDetails);

					foreach (var term in tmpData!)
					{
						term.Plazo = int.Parse(termText);
					}

					result.plazos.AddRange(tmpData);
				}
				else
				{
					result.fechaarranque = DateTime.ParseExact(item.First!.Value<string>()!, "dd-MM-yyyy", CultureInfo.InvariantCulture);
				}
			}
		}
		catch (Exception)
		{
			result = null!;
		}

		return result;
	}

	private bool ValidateEntity(DateTime fecha, CtlCatalogoPlazo entity)
	{
		var existRecord = _catalogosDbContext.CtlBonificacionCarteras.FirstOrDefault(i =>
						i.IdPlazo == entity.IdPlazo &&
						i.FechaArranque.Year == fecha.Year &&
						i.FechaArranque.Month == fecha.Month &&
						i.FechaArranque.Day == fecha.Day);

		if (existRecord == null)
			return true;
		else
		{
			return false;
		}
	}
}
