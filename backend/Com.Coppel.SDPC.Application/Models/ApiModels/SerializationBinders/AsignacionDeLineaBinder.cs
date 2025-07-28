using Newtonsoft.Json.Serialization;
using System;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.SerializationBinders;

#nullable disable
public class AsignacionDeLineaBinder : ISerializationBinder
{
	public void BindToName(Type serializedType, out string assemblyName, out string typeName)
	{
		assemblyName = serializedType.Assembly.FullName ?? string.Empty;
		typeName = serializedType.FullName ?? string.Empty;
	}

	public Type BindToType(string assemblyName, string typeName)
	{
		return typeName.CompareTo("BonificacionesDataVM") != 0 ? null : Type.GetType($"{typeName}, {assemblyName}");
	}
}
