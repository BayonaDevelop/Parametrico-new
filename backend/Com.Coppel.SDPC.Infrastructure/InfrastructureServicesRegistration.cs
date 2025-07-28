using Com.Coppel.SDPC.Application.Infrastructure.Services;
using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Application.Infrastructure.Services.ApiClients.TasasDeInteres;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Com.Coppel.SDPC.Infrastructure.Services;
using Com.Coppel.SDPC.Infrastructure.Services.ApiClients;
using Com.Coppel.SDPC.Infrastructure.Services.ApiClients.TasasDeInteres;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Coppel.SDPC.Infrastructure;

public static class InfrastructureServicesRegistration
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		/// Registro de DbContext
		services.AddDbContext<CatDbContext>();
		services.AddDbContext<CatalogosDbContext>();
		services.AddDbContext<CarterasDbContext>();
		services.AddDbContext<ControlTiendasDbContext>();
		services.AddDbContext<Emision20DbContext>();
		services.AddDbContext<ListadosCobranzaDbContext>();

		/// Servicios de API
		_ = services.AddTransient<IServiceApiToken, ServiceApiToken>();
		_ = services.AddTransient<IServiceApiSalarioMinimo, IServiceApiSalarioMinimo>();
		_ = services.AddTransient<IServiceApiDecrementoLineaCredito, ServiceApiDecrementoLineaCredito>();
		_ = services.AddTransient<IServiceApiTasaDeInteres, ServiceApiTasaDeInteres>();
		_ = services.AddTransient<IServiceApiTasaDeInteresMoratorio, ServiceApiTasaDeInteresMoratorio>();
		_ = services.AddTransient<IServiceApiTasaDeInteresMoratorioPrestamoDia, ServiceApiTasaDeInteresMoratorioPrestamoDia>();
		_ = services.AddTransient<IServiceApiFactorDeSaturacion, ServiceApiFactorDeSaturacion>();
		_ = services.AddTransient<IServiceApiTasaDeInteresPrestamo, ServiceApiTasaDeInteresPrestamo>();
		_ = services.AddTransient<IServiceApiBonificaciones, ServiceApiBonificaciones>();
		_ = services.AddTransient<IServiceApiConversionLineasCredito, ServiceApiConversionLineasCredito>();
		_ = services.AddTransient<IServiceApiAsignacionDeLinea, ServiceApiAsignacionDeLinea>();
		_ = services.AddTransient<IServiceApiAsignacionDeLineaGrupo, ServiceApiAsignacionDeLineaGrupo>();

		/// Servicios de Procesamiento de datos
		_ = services.AddTransient<IServicePuntosDeConsumo, ServicePuntosDeConsumo>();


		return services;
	}
}
