using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Com.Coppel.SDPC.Infrastructure.Services.ApiClients;

public class ServiceApiConversionLineasCredito : ServiceUtils, IServiceApiConversionLineasCredito
{
	public ServiceApiConversionLineasCredito() : base(new CatalogosDbContext()) { }

	public ApiResultType GetData(string token, PuntoDeConsumoVM puntoDeConsumo)
	{
		ApiResultType result;
		this.jwtToken = token;

		try
		{
			dynamic response = GetDataFromApi(puntoDeConsumo);

			if (response == null)
			{
				return ApiResultType.NO_DATA;
			}

			DateTime fechaArranque = GetDate((JValue)response.SelectToken("infoProgramacion[0]['fechaArranque']"));

			JArray perfilesJson = (JArray)response.SelectToken("iperfil");
			JToken valoresJson = response.SelectToken("valores");
			JArray gruposJson = (JArray)response.SelectToken("igrupo");

			CtlValidacionesLrc validacion = GetPerfiles(perfilesJson, valoresJson, fechaArranque);
			List<CtlPerfilRiesgoLrc> grupos = GetGroups(gruposJson, fechaArranque);

			if (ValidateLrc(fechaArranque))
			{
				if (validacion.FechaArranque.Year == 1900)
				{
					validacion.Estatus = (int)EstatusType.Finalizado;
				}

				_catalogosDbContext.CtlValidacionesLrcs.Add(validacion);
				result = ApiResultType.SUCCESS;
			}
			else
			{
				result = ApiResultType.ALREADY_SAVED;
			}

			if (ValidatePerfiles(fechaArranque))
			{
				if (grupos[0].FechaArranque.Year == 1900)
				{
					foreach (var item in grupos)
					{
						item.Estatus = (int)EstatusType.Finalizado;
					}
				}
				_catalogosDbContext.CtlPerfilRiesgoLrcs.AddRange(grupos);
			}
			else
			{
				result = ApiResultType.ALREADY_SAVED;
			}

			_catalogosDbContext.SaveChanges();
		}
		catch (SqlException)
		{
			result = ApiResultType.TABLE_NOT_FOUND;
		}

		return result;
	}

	private static DateTime GetDate(JValue jsonValue)
	{
		string[] dateParts = jsonValue.Value!.ToString()!.Split("-");

		return new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]), 0, 0, 0, DateTimeKind.Local);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6608:Prefer indexing instead of \"Enumerable\" methods on types implementing \"IList\"", Justification = "In this case whe prefer Enumerable")]
	private static CtlValidacionesLrc GetPerfiles(JArray perfilesJson, JToken values, DateTime fechaArranque)
	{
		CtlValidacionesLrc result = new();
		result = GetValues(result, values);
		result.FechaAlta = DateTime.Now;
		result.FechaArranque = fechaArranque;
		result.Estatus = (int)EstatusType.PorActualizar;

		try
		{
			foreach (JToken item in perfilesJson)
			{
				JEnumerable<JToken> perfiles = item.Children();

				foreach (JToken perfil in perfiles)
				{
					JProperty perfilObject = (JProperty)perfil;
					int numeroPerfil = int.Parse(perfilObject.Path.Split('.').Last().Replace("perfil", ""));

					if (numeroPerfil == 1)
					{
						result.PrcPerfil1 = GetPerfilValue(perfil);
					}
					else
					{
						result.PrcPerfil2 = GetPerfilValue(perfil);
					}
				}
			}
		}
		catch (Exception)
		{
			result = new();
		}

		return result;
	}

	private static int GetPerfilValue(JToken items)
	{
		int result = 0;

		foreach (var values in items.Children())
		{
			foreach (JToken value in values)
			{
				result = value.SelectToken("prcPerfil")!.Value<int>();
			}
		}

		return result;
	}

	private static CtlValidacionesLrc GetValues(CtlValidacionesLrc entity, JToken items)
	{
		JToken values = items.SelectToken("[0]")!;

		entity.PrcCalcularCsa = values.SelectToken("prcCalcularCSA")!.Value<int>();
		entity.NumMesesCalcularLrc = values.SelectToken("numMesesCalcularLRC")!.Value<int>();
		entity.Puntualidad = values.SelectToken("Puntualidad")!.Value<string>()!;
		entity.ImporteVencido = values.SelectToken("importeVencido")!.Value<int>();
		entity.NumMesesMinimoPrimeraCompra = values.SelectToken("numMesesMinimoPrimeraCompra")!.Value<int>();
		entity.TopeEdadMaxima = values.SelectToken("topeEdadMaxima")!.Value<int>();

		return entity;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6608:Prefer indexing instead of \"Enumerable\" methods on types implementing \"IList\"", Justification = "In this case whe prefer Enumerable")]
	private static List<CtlPerfilRiesgoLrc> GetGroups(JArray values, DateTime fechaArranque)
	{
		List<CtlPerfilRiesgoLrc> result = [];

		try
		{
			foreach (JToken item in values)
			{
				JEnumerable<JToken> groups = item.Children();
				int numProceso = 0;

				foreach (JToken group in groups)
				{
					JProperty groupObject = (JProperty)group;

					if (groupObject.Path.Split(".").Last().Contains("numProceso"))
					{
						numProceso = groupObject.Children().First().Value<int>();
					}
					else
					{
						int groupId = int.Parse(groupObject.Path.Split('.').Last().ToString().Replace("grupo", ""));
						foreach (var detail in group.Children())
						{
							result.Add(GetValuesOfItem(detail, groupId, fechaArranque));
						}
					}
				}

				foreach (CtlPerfilRiesgoLrc perfil in result)
				{
					perfil.NumProceso = numProceso;
				}
			}
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}

		return result;
	}

	private static CtlPerfilRiesgoLrc GetValuesOfItem(JToken item, int groupId, DateTime fechaArranque)
	{
		CtlPerfilRiesgoLrc result = new()
		{
			FechaAlta = DateTime.Now,
			FechaArranque = fechaArranque,
			Estatus = (int)EstatusType.PorActualizar,
			Grupo = groupId,
			EdadMinimaPerfil = item.Children().First().SelectToken("edadMinimaPerfil")!.Value<int>(),
			EdadMaximaPerfil = item.Children().First().SelectToken("edadMaximaPerfil")!.Value<int>(),
			TopeMinimoPerfil = item.Children().First().SelectToken("topeMinimoPerfil")!.Value<double>(),
			TopeMaximoPerfil = item.Children().First().SelectToken("topeMaximoPerfil")!.Value<double>(),
			SmcminimoPerfil = item.Children().First().SelectToken("SMCMinimoPerfil")!.Value<double>(),
			SmcmaximoPerfil = item.Children().First().SelectToken("SMCMaximoPerfil")!.Value<double>()
		};

		return result;
	}

	private bool ValidateLrc(DateTime fechaArranque)
	{
		var existRecord = _catalogosDbContext.CtlValidacionesLrcs.FirstOrDefault(i =>
						i.FechaArranque.Year == fechaArranque.Year &&
						i.FechaArranque.Month == fechaArranque.Month &&
						i.FechaArranque.Day == fechaArranque.Day);

		if (existRecord == null)
		{
			return true;
		}

		return false;
	}

	private bool ValidatePerfiles(DateTime fechaArranque)
	{
		var existRecord = _catalogosDbContext.CtlPerfilRiesgoLrcs.FirstOrDefault(i =>
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
