using System.Collections.Generic;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.AsignacionDeLinea;

public class TipoLineaVM
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "We nneed to break Naming Styles to map data from Api")]
	public int idu_lineadecredito { get; set; }
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "We nneed to break Naming Styles to map data from Api")]
	public string tipoLinea { get; set; } = string.Empty;
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "We nneed to break Naming Styles to map data from Api")]
	public List<PerfilVM> perfiles { get; set; } = [];
}
