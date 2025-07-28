namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.Bonificaciones;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class BonificacionPlazoVM
{
	public int Plazo { get; set; }
	public int diastranscurridos { get; set; }
	public decimal porcentajebonificacion { get; set; }
}
