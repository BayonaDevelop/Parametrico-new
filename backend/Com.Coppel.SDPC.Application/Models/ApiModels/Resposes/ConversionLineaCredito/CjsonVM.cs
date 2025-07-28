using System.Collections.Generic;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.ConversionLineaCredito;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class CjsonVM
{
	public List<ValorVM> valores { get; set; } = [];

	public List<KeyValuePair<string, List<GroupVM>>> igrupo { get; set; } = [];
}
