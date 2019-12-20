using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Security.Infrastructure
{
	internal static class CryptographyHelper
	{
		private static readonly SHA512Managed _hasher = new SHA512Managed();

		public static byte[] GenerateSalt(int bytesLength)
		{
			var bytes = new byte[bytesLength];
			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(bytes);
			}

			return bytes;
		}
		public static byte[] ComputeHash(IEnumerable<byte> input, byte[] salt)
		{
			var data = input.Concat(salt).ToArray();
			return _hasher.ComputeHash(data);
		}

		public static byte[] ComputeHash(byte[] input)
		{
			return _hasher.ComputeHash(input);
		}

		public static byte[] Encrypt(byte[] input, string privateKey)
		{
			var encryptEngine = new Pkcs1Encoding(new RsaEngine());
			using (var reader = new StringReader(privateKey))
			{
				var keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
				encryptEngine.Init(true, keyPair.Private);
			}

			return encryptEngine.ProcessBlock(input, 0, input.Length);
		}

		public static byte[] Decrypt(byte[] input, string publicKey)
		{
			var decryptEngine = new Pkcs1Encoding(new RsaEngine());
			using (var reader = new StringReader(publicKey))
			{
				var keyParameter = (AsymmetricKeyParameter)new PemReader(reader).ReadObject();
				decryptEngine.Init(false, keyParameter);
			}

			return decryptEngine.ProcessBlock(input, 0, input.Length);
		}

		public static bool AreHashesEqual(byte[] hash1, byte[] hash2)
		{
			if (hash1.Length != hash2.Length)
			{
				return false;
			}

			for (int i = 0; i < hash1.Length; ++i)
			{
				if (hash1[i] != hash2[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
