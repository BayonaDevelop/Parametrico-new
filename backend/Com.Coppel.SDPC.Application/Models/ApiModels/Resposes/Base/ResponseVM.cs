namespace Com.Coppel.SDPC.Application.Models.ApiModels.Resposes.Base;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "To map data is neccessary to break down Naming Styles")]
public class ResponseVM
{
	public MetaVM meta { get; set; } = new();
	public dynamic data { get; set; } = new { };
}
