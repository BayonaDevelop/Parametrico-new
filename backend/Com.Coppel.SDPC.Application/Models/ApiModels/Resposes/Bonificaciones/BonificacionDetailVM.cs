using System;
using System.Collections.Generic;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.Bonificaciones;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class BonificacionDetailVM
{
	public DateTime fechaarranque { get; set; }
	public List<BonificacionPlazoVM> plazos { get; set; } = [];
}
