using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JahanJooy.Common.Util.Security;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Security
{
	/// <summary>
	/// Code inspired by:
	/// http://www.dijksterhuis.org/creating-salted-hash-values-in-c/
	/// </summary>
	public static class PasswordHashUtil
	{
		private const int SaltLength = 8;
        private static readonly HashAlgorithm HashAlgorithm = new SHA256Managed();

		public static void SetSaltAndHash(User user, string password)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentNullException(nameof(password));

			var passwordBytes = Encoding.UTF8.GetBytes(password);
			var saltBytes = CryptoRandomNumberUtil.GetBytes(SaltLength, true);

			byte[] hashBytes = ComputeHash(passwordBytes, saltBytes);

			user.PasswordHash = Convert.ToBase64String(hashBytes);
			user.PasswordSalt = Convert.ToBase64String(saltBytes);
		}

		public static bool VerifySaltAndHash(User user, string password)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentNullException(nameof(password));

			if (string.IsNullOrWhiteSpace(user.PasswordHash) || string.IsNullOrWhiteSpace(user.PasswordSalt))
				throw new ArgumentException("Incorrect hash / salt in User object");

			var passwordBytes = Encoding.UTF8.GetBytes(password);
			var saltBytes = Convert.FromBase64String(user.PasswordSalt);
			var newHashBytes = ComputeHash(passwordBytes, saltBytes);

			var hashBytes = Convert.FromBase64String(user.PasswordHash);

			if (newHashBytes == null || hashBytes.Length < 1 || hashBytes.Length != newHashBytes.Length)
				return false;

			return !hashBytes.Where((t, i) => t != newHashBytes[i]).Any();
		}

		private static byte[] ComputeHash(byte[] data, byte[] salt)
		{
			var dataAndSalt = new byte[data.Length + salt.Length];

			Array.Copy(data, dataAndSalt, data.Length);
			Array.Copy(salt, 0, dataAndSalt, data.Length, salt.Length);
 
			return HashAlgorithm.ComputeHash(dataAndSalt);
		}
	}
}