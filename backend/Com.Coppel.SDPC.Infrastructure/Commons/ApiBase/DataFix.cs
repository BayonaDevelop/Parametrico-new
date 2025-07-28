using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes;
using Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;
using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Core.Catalogos;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
namespace Com.Coppel.SDPC.Infrastructure.Commons.ApiBase;

public static class DataFix
{
	private const string DATE_FORMAT = "dd-MM-yyyy";

	public static List<CtlFactoresSaturacionCartera> ConvertFactoresDeSaturacionToEntities(List<FactorDeSaturacionVM> result, string cartera)
	{
		try
		{
			List<CtlFactoresSaturacionCartera> factoresSaturacion = [];
			foreach (FactorDeSaturacionVM item in result)
			{
				DateTime fechaArranque = DateTime.ParseExact(item.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
				CtlFactoresSaturacionCartera regFactore = new()
				{
					FechaAlta = DateTime.Now,
					FechaActualizacion = null,
					Cartera = cartera,
					Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
					Plazo = item.plazo,
					FactorNormal = item.factornormal,
					FactorEspecial = item.factorespecial,
					FactorInicial = item.factorinicial,
					FactorMinima = item.factorminima,
					FechaArranque =  fechaArranque
				};
				factoresSaturacion.Add(regFactore);

			}
			return factoresSaturacion;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static CtlSalarioMinimoCp ConvertSalarioMinimoCpToEntity(SalarioMinimoVM result)
	{
		try
		{
			DateTime fechaArranque = DateTime.ParseExact(result.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
			return new CtlSalarioMinimoCp
			{
				FechaAlta = DateTime.Now,
				FechaActualizacion = null,
				Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
				EstatusCl = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
				NombreSalario = result.nombreSalario,
				ValorSalario = result.valorSalario,
				FechaArranque = DateTime.ParseExact(result.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture)
			};
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static CtlSalarioMinimoLc ConvertSalarioMinimoLcToEntity(SalarioMinimoVM result)
	{
		try
		{
			DateTime fechaArranque = DateTime.ParseExact(result.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
			return new CtlSalarioMinimoLc
			{
				FechaAlta = DateTime.Now,
				FechaActualizacion = null,
				Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
				NombreSalario = result.nombreSalario,
				ValorSalario = result.valorSalario,
				FechaArranque = fechaArranque
			};
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static List<CtlTasasInteresCp> ConvertTdiToEntity(List<TasaInteresVM> data, string cartera)
	{
		try
		{
			DateTime fechaArranque = DateTime.ParseExact(data[0].fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
			List<CtlTasasInteresCp> result = [];

			foreach (TasaInteresVM item in data)
			{
				result.Add(
					new CtlTasasInteresCp
					{
						FechaAlta = DateTime.Now,
						FechaActualizacion = null,
						Cartera = cartera,
						Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
						Plazo = item.plazo,
						TasaDeInteres = item.tasaDeInteres,
						FechaArranque = fechaArranque
					}
				);
			}

			return result;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static List<CtlInteresMoratorioDcp> ConvertTdimPorDiaToEntities(JArray response)
	{
		try
		{
			List<CtlInteresMoratorioDcp> conversionMoratorioDCP = [];

			List<TasaInteresMoratoriaPrestamosDiaVM> groupsList = [];

			JArray jsonResponse = response;
			foreach (JToken item in jsonResponse)
			{
				JToken[] values = [.. item["tasaInteresDiario"]!];

				foreach (JToken details in values)
				{
					List<JToken> detail = [.. details.Values()];

					groupsList.Add(new TasaInteresMoratoriaPrestamosDiaVM
					{
						diasTranscurridos = detail[0].Value<int>(),
						tasaMoratoria = detail[1].Value<decimal>(),
						fechaIns = item["fechains"]!.ToString(),
						userModifico = item["usermodifico"]!.ToString()
					});
				}
			}

			foreach (TasaInteresMoratoriaPrestamosDiaVM item in groupsList)
			{
				DateTime fechaArranque = DateTime.ParseExact(item.fechaIns, DATE_FORMAT, CultureInfo.InvariantCulture);

				CtlInteresMoratorioDcp regConversionDCP = new()
				{
					FechaAlta = DateTime.Now,
					Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
					EstatusCl = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
					DiasTranscurridos = item.diasTranscurridos,
					TasaMoratoria = item.tasaMoratoria,
					Fechains = fechaArranque,
					Usermodifico = item.userModifico,
				};
				conversionMoratorioDCP.Add(regConversionDCP);
			}

			return conversionMoratorioDCP;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static List<CtlTasasInteresCpc> ConvertTdiMueblesToEntities(List<TasaInteresMueblesVM> result)
	{
		try
		{
			List<CtlTasasInteresCpc> TasasInteresMuebles = [];
			foreach (TasaInteresMueblesVM item in result)
			{
				foreach (TasaInteresVM subitem in item.plazos)
				{
					DateTime fechaActual = DateTime.Now;
					DateTime fechaArranque = DateTime.ParseExact(item.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
					CtlTasasInteresCpc regTasasInteresMuebles = new()
					{
						FechaAlta = fechaActual,
						FechaActualizacion = null,
						Cartera = "Muebles",
						Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
						Ciudad = item.ciudad,
						Articulo = item.articulo,
						Plazo = subitem.plazo,
						TasaDeInteres = subitem.tasaDeInteres,
						FechaArranque = fechaArranque
					};
					TasasInteresMuebles.Add(regTasasInteresMuebles);
				}
			}
			return TasasInteresMuebles;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static List<CtlTasasInteresCp> ConvertTdiToEntities(List<TasaInteresVM> result, string cartera)
	{
		try
		{
			List<CtlTasasInteresCp> TasasInteres = [];
			foreach (TasaInteresVM item in result)
			{
				DateTime fechaArranque = DateTime.ParseExact(item.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
				CtlTasasInteresCp regTasas = new()
				{
					FechaAlta = DateTime.Now,
					FechaActualizacion = null,
					Cartera = cartera,
					Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
					Plazo = item.plazo,
					TasaDeInteres = item.tasaDeInteres,
					FechaArranque = fechaArranque
				};
				TasasInteres.Add(regTasas);
			}
			return TasasInteres;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw	;
		}
	}

	public static CtlInteresMoratorioporCartera ConvertTdimToEntity(TasaInteresMoratorioVM result)
	{
		try
		{
			DateTime fechaArranque = DateTime.ParseExact(result.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);

			return new CtlInteresMoratorioporCartera
			{
				FechaAlta = DateTime.Now,
				Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
				EstatusCl = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
				TasaTipoCiudad1 = result.tasatipociudad1,
				TasaTipoCiudad2 = result.tasatipociudad2,
				FechaArranque = DateTime.ParseExact(result.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture)
			};
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static List<CtlTasaInteresCpprestamo> ConvertTdiPrestamosToEntities(List<TasaInteresPrestamoVM> result, string cartera)
	{
		try
		{
			List<CtlTasaInteresCpprestamo> tasaInteresPrestamos = [];
			foreach (TasaInteresPrestamoVM item in result)
			{
				DateTime fechaArranque = DateTime.ParseExact(item.fechaArranque, DATE_FORMAT, CultureInfo.InvariantCulture);
				CtlTasaInteresCpprestamo regTasasPrestamo = new()
				{
					FechaAlta = DateTime.Now,
					FechaActualizacion = null,
					Cartera = "Prestamos",
					Estatus = fechaArranque.Year == 1900 ? (int)EstatusType.Finalizado : (int)EstatusType.PorActualizar,
					Puntualidad = cartera,
					Grupo = item.grupo,
					PuntajeInicial = item.puntajeInicial,
					PuntajeFinal = item.puntajeFinal,
					Plazo = item.plazo,
					TasaDeInteres = item.tasaDeInteres,
					FechaArranque = fechaArranque
				};
				tasaInteresPrestamos.Add(regTasasPrestamo);
			}
			return tasaInteresPrestamos;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}
}
