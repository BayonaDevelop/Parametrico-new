using System.Runtime.Serialization;

namespace Com.Coppel.SDPC.Application.Models.Reports.FactoresSaturacion;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "For mapping data we need to break Naming Styles")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "For mapping data we need to break PascalCase")]
public class cat_factorescarterasVM
{
  [DataMember(Name = "idu_cartera")]
  public string IduCartera {  get; set; } = string.Empty;

  [DataMember(Name = "clv_plazo")]
  public string ClvPlazo {  get; set; } = string.Empty;

  [DataMember(Name = "prc_factor")]
  public string PrcFactor {  get; set; } = string.Empty;

  [DataMember(Name = "clv_lineacreditoespecial")]
  public string ClvLineaCreditoEspecial {  get; set; } = string.Empty;

}
