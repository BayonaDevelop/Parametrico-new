using Newtonsoft.Json.Serialization;
using System;

namespace Com.Coppel.SDPC.Application.Models.ApiModels.SerializationBinders;

#nullable disable
public class ParametroAutenticacionSerializationBinder : ISerializationBinder
{
	private static readonly DefaultSerializationBinder Binder = new();

	public void BindToName(Type serializedType, out string assemblyName, out string typeName)
	{
		Binder.BindToName(serializedType, out assemblyName, out typeName);
	}

	public Type BindToType(string assemblyName, string typeName)
	{
		return typeName.CompareTo("CtlParametrosautenticacion") != 0 ? null : Binder.BindToType(assemblyName, typeName);
	}
}
