namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class SalarioMinimoVM
{
	public string nombreSalario { get; set; } = string.Empty;
	public decimal valorSalario { get; set; }
	public string fechaArranque { get; set; } = string.Empty;
}
