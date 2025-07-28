namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.Bonificaciones;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class BonificacionesResponseVM
{
	public int iestado { get; set; }
	public string cmensaje { get; set; } = string.Empty;
	public dynamic cjson { get; set; } = new { };
}
