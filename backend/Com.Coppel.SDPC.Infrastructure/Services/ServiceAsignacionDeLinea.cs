using Com.Coppel.SDPC.Application.Commons.Files;
using Com.Coppel.SDPC.Application.Infrastructure.Services;
using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Application.Models.Services;
using Com.Coppel.SDPC.Core.Catalogos;
using Com.Coppel.SDPC.Infrastructure.Commons;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;

namespace Com.Coppel.SDPC.Infrastructure.Services;

public class ServiceAsignacionDeLinea(IServiceApiAsignacionDeLinea serviceApiAsignacionDeLinea, IServiceEmail serviceEmail, IServiceExcel serviceExcel , IServicePdfAignacionLineasCredito servicePdf) : ServiceUtils(new CatalogosDbContext()), IServiceAsignacionDeLinea
{
	private readonly CatalogosDbContext _catalogosDbContext = new();
	private readonly CarterasDbContext _carterasDbContext = new();
	private readonly DateTime _today = DateTime.Now;
	private readonly TestDatesVM _testDates;
	private readonly string _contactsOfCarteraCentral;
	private readonly PuntoDeConsumoVM _puntoDeConsumo;
	private readonly List<int> _listOfValidStatusToBeCensed;

	private List<dynamic> _parameters = [];
	private List<CatParametrosasignacionlinea> _catParametrosasignacionlineas = [];
	private List<CatParametrosasignacionlineaHistorial> _catParametrosasignacionlineasOldInCatalogos = [];
	private List<CatParametrosasignacionlineaHistorial> _catParametrosasignacionlineasOldInCarteras = [];
	private List<CatParametrosasignacionlinea> _catParametrosasignacionlineaBeforeUpdate = [];

	public bool ProcessDaily(string token)
	{
		return true;
	}

	public bool ProcesssAfter20(string token)
	{
		throw new NotImplementedException();
	}
}
