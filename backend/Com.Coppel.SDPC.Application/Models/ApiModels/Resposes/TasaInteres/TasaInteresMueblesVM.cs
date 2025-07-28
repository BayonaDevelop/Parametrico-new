using System.Collections.Generic;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class TasaInteresMueblesVM
{
	public int ciudad { get; set; }
	public string articulo { get; set; } = string.Empty;
	public string fechaArranque { get; set; } = string.Empty;
	public List<TasaInteresVM> plazos { get; set; } = [];
}
