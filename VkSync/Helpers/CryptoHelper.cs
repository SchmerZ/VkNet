using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VkSync.Helpers
{
	public static class CryptoHelper
	{
		public static string Encrypt(string data, string key, string iv)
		{
			if (string.IsNullOrEmpty(data))
				return null;

			var keyBytes = GetBytes(key, 16);
			var ivBytes = GetBytes(iv, 16);

			return Encrypt(data, keyBytes, ivBytes);
		}

		public static string Encrypt(string data, byte[] key, byte[] iv)
		{
			if (string.IsNullOrEmpty(data))
				return null;

			var byteData = Encoding.UTF8.GetBytes(data);

			var byteEncryptedData = Encrypt(byteData, key, iv);

			return Convert.ToBase64String(byteEncryptedData);
		}

		public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
		{
			using (var rijndael = Rijndael.Create())
			using (var encryptor = rijndael.CreateEncryptor(key, iv))
			{
				return Crypt(data, encryptor);
			}
		}

		public static string Decrypt(string data, string key, string iv)
		{
			if (string.IsNullOrEmpty(data))
				return null;

			var keyBytes = GetBytes(key, 16);
			var ivBytes = GetBytes(iv, 16);

			return Decrypt(data, keyBytes, ivBytes);
		}

		public static string Decrypt(string data, byte[] key, byte[] iv)
		{
			var byteData = Convert.FromBase64String(data);

			var byteDecryptedData = Decrypt(byteData, key, iv);

			return Encoding.UTF8.GetString(byteDecryptedData);
		}

		public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
		{
			using (var rijndael = Rijndael.Create())
			using (var decryptor = rijndael.CreateDecryptor(key, iv))
			{
				return Crypt(data, decryptor);
			}
		}

		private static byte[] Crypt(byte[] data, ICryptoTransform cryptoTransform)
		{
			var result = new byte[0];

			try
			{
				using (var memoryStream = new MemoryStream())
				using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(data, 0, data.Length);
					cryptoStream.FlushFinalBlock();

					result = memoryStream.ToArray();
				}
			}
			catch
			{ }

			return result;
		}

		public static byte[] GetBytes(string data, int bytesCount)
		{
			var salt = Encoding.UTF8.GetBytes("password");

			var rfc2898DeriveBytes = new Rfc2898DeriveBytes(data, salt);

			return rfc2898DeriveBytes.GetBytes(bytesCount);
		}
	}
}