using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Com.Coppel.SDPC.Infrastructure.Commons;

public static class AesCipher
{
	public static string EncryptFile(string filePath)
	{
		try
		{
			string decriptedText = File.ReadAllText(filePath);
			byte[] encrypted;
			string result = string.Empty;
			string path = Path.GetFullPath("./");
			string encriptedFilePath = Path.GetFullPath(Path.Combine(path, "connections.txt"));

			using (Aes aesAlg = Aes.Create())
			{
				if (File.Exists(encriptedFilePath))
				{
					File.Delete(encriptedFilePath);
				}

				aesAlg.Key = Convert.FromBase64String(Utils.GetConfiguration.GetValue<string>("appSettings:apiToken")!);
				aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV should be the same size as the block size
				aesAlg.Mode = CipherMode.CBC; // Set the mode to CBC
				aesAlg.Padding = PaddingMode.Zeros; // Use PKCS7 padding

				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				using MemoryStream msEncrypt = new();
				using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
				using (StreamWriter swEncrypt = new(csEncrypt))
				{
					swEncrypt.Write(decriptedText);
				}

				if (!File.Exists(encriptedFilePath))
				{
					encrypted = msEncrypt.ToArray();
					result = Convert.ToBase64String(encrypted);
					File.WriteAllText(encriptedFilePath, result);
				}
			}

			return result;
		}
		catch (Exception)
		{
			Debug.WriteLine("");
			throw;
		}
	}

	public static string DecryptFile(string filePath)
	{
		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException("El archivo de conexiones no existe", filePath);
		}

		try
		{
			byte[] cipherText;
			var tmp = new FileInfo(filePath);
			if (Utils.IsFileLocked(tmp))
			{
				return string.Empty;
			}

			cipherText = Convert.FromBase64String(File.ReadAllText(filePath));
			StringBuilder plaintext = new();

			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = Convert.FromBase64String(Utils.GetConfiguration.GetValue<string>("appSettings:apiToken")!);
				aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV should be the same size as the block size
				aesAlg.Mode = CipherMode.CBC; // Set the mode to CBC
				aesAlg.Padding = PaddingMode.Zeros; // Use PKCS7 padding

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				using MemoryStream msDecrypt = new(cipherText);
				using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
				using StreamReader srDecrypt = new(csDecrypt);
				
				string line = string.Empty;
				while ((line = srDecrypt.ReadLine()!) != null)
				{
					plaintext.Append(line);
				}
			}
			return plaintext.ToString();
		}
		catch (Exception)
		{
			Debug.WriteLine("No se pudo sesncriptar el archivo de conexiones");
			throw ;
		}
	}
}
