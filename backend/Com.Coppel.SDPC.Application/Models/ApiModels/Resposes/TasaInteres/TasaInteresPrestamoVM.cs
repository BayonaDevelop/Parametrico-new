﻿namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class TasaInteresPrestamoVM
{
	public string puntualidad { get; set; } = string.Empty;
	public string grupo { get; set; } = string.Empty;
	public int puntajeInicial { get; set; }
	public int puntajeFinal { get; set; }
	public int plazo { get; set; }
	public decimal tasaDeInteres { get; set; }
	public string fechaArranque { get; set; } = string.Empty;
}
