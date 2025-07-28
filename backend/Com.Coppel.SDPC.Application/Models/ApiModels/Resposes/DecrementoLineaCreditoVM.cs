﻿namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class DecrementoLineaCreditoVM
{
	public int iestado { get; set; }
	public string cmensaje { get; set; } = string.Empty;
	public int numtipolc { get; set; }
	public string nomtipolinea { get; set; } = string.Empty;
	public decimal numtopelineaminima { get; set; }
	public string nompuntualidad { get; set; } = string.Empty;
	public decimal numeficienciahistorica { get; set; }
	public decimal numdecremento { get; set; }
	public string fechaarranque { get; set; } = string.Empty;
}
