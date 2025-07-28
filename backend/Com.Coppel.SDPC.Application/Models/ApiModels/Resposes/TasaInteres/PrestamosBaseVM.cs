using System.Collections.Generic;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;

public class PrestamosBaseVM
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
	public string fechaarranque { get; set; } = string.Empty;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
	public List<TasaInteresPrestamoVM> data { get; set; } = [];
}
