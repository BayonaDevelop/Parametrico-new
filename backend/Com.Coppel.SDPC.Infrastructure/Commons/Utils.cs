using Com.Coppel.SDPC.Application.Models.Enums;
using Com.Coppel.SDPC.Application.Models.Persistence;
using Com.Coppel.SDPC.Infrastructure.Commons.DataContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Com.Coppel.SDPC.Infrastructure.Commons;

public static class Utils
{
	private static readonly string _basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!.Replace("file:\\", "")!;

	private static readonly CatalogosDbContext _dbContext = new();

	public static IConfiguration GetConfiguration
	{
		get
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
			.Build();

			return builder;
		}
	}

	public static bool IsFileLocked(FileInfo file)
	{
		try
		{
			using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			stream.Close();
		}
		catch (IOException)
		{
			return true;
		}

		//file is not locked
		return false;
	}

	public static string GetProfileName()
	{
		string result = string.Empty;
		Profile profile = (Profile)short.Parse(GetConfiguration.GetValue<string>("ASPNETCORE_ENVIRONMENT")!);

		switch (profile)
		{
			case Profile.PRODUCTION:
				result = "Production";
				break;

			case Profile.DEVELOP:
				result = "Develop";
				break;

			case Profile.TEST:
				result = "Test";
				break;
		}

		return result;
	}

	public static bool IsInProduction()
	{
		Profile profile = (Profile)short.Parse(GetConfiguration.GetValue<string>("ASPNETCORE_ENVIRONMENT")!);
		return profile == Profile.PRODUCTION;
	}

	public static bool IsInTesting()
	{
		Profile profile = (Profile)short.Parse(GetConfiguration.GetValue<string>("ASPNETCORE_ENVIRONMENT")!);
		return profile == Profile.TEST;
	}

	private static List<KeyValuePair<string, string>> GetValuesFromJson(JObject? jobjet)
	{
		List<KeyValuePair<string, string>> connections = [];
		if (jobjet != null)
		{
			var json = jobjet.SelectToken("ConnectionStrings");
			if (json != null)
			{
				foreach (JToken item in json.ToList())
				{
					JProperty jProperty = item.ToObject<JProperty>()!;

					if (jProperty != null)
					{
						DatabaseType database = (DatabaseType)Enum.Parse(typeof(DatabaseType), jProperty.Name);
						string dataBasePath = $"{_basePath}\\Assets\\Databases\\{database}";

						var connection = (IsInTesting()) ?
							new KeyValuePair<string, string>(jProperty.Name, $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dataBasePath}.mdf;Integrated Security=True") :
							new KeyValuePair<string, string>(jProperty.Name, jProperty.Value.ToString());

						connections.Add(connection);
					}
				}
			}
		}

		return connections;
	}

	public static List<KeyValuePair<string, string>> GetConnectionStrings()
	{
		List<KeyValuePair<string, string>> connections = [];
		string basePath = $"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!.Replace("file:\\", "")!}";
		string connectionsFilePath = $"{basePath}\\connections.txt";

		try
		{
			IsFileLocked(new FileInfo(connectionsFilePath));
			string decryptedText = AesCipher.DecryptFile(connectionsFilePath);

			var jobjet = JObject.Parse(decryptedText);
			connections = GetValuesFromJson(jobjet);
		}
		catch (Exception)
		{
			Debug.WriteLine("No jala");
		}

		return connections;		
	}

	public static PuntoDeConsumoVM GetPuntoConsumo(int id)
	{
		try
		{
			return _dbContext.CtlPuntosdeconsumos.AsNoTracking().Select(i => new PuntoDeConsumoVM
			{
				IdFuncionalidad = i.IdFuncionalidad,
				Flag = i.Flag!.Value,
				AllowAfter20 = i.AllowAfter20!.Value,
				NomFuncionalidad = i.NomFuncionalidad!,
				RutaServicio = i.RutaServicio!,
				NomTbDestino = i.NomTbDestino!
			})
		.FirstOrDefault(i => i.IdFuncionalidad == id)!;
		}
		catch (Exception)
		{
			return null!;
		}
	}
}
