namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.TasaInteres;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class TasaInteresVM
{
	public int plazo { get; set; }
	public decimal tasaDeInteres { get; set; }
	public string fechaArranque { get; set; } = string.Empty;
}

