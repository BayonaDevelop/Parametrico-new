namespace Com.Coppel.SDPC.Application.Infrastructure.Services
{
	public interface IServiceAsignacionDeLineaGrupo
  {
    bool ProcessParams(string token);

		bool ProcessCarterasAfter20(string token);
	}
}
