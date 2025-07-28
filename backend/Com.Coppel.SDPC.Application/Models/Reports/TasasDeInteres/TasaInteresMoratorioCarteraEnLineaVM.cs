using System.Runtime.Serialization;

namespace Com.Coppel.SDPC.Application.Models.Reports.TasasDeInteres;

public class TasaInteresMoratorioCarteraEnLineaVM
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "For mapping data we need to break Naming Styles")]
	[DataMember(Name = "IduCiudad")]	
	public short? idu_ciudad { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "For mapping data we need to break Naming Styles")]
	[DataMember(Name = "IduCuenta")]	
	public short? idu_cuenta { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "For mapping data we need to break Naming Styles")]
	[DataMember(Name = "NumPorcentajeint")]	
	public int? num_porcentajeint { get; set; }

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "For mapping data we need to break Naming Styles")]
	[DataMember(Name = "FecMovto")]	
	public DateTime? fec_movto { get; set; }
}
