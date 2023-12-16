using System;
using System.Security.Cryptography;
using System.Text;
using JahanJooy.Common.Util.Security;
using JahanJooy.RealEstateAgency.Domain.Users;

namespace JahanJooy.RealEstateAgency.Util.Security
{
    public class PasswordHashUtil
    {
        private const int SaltLength = 8;
        private static readonly HashAlgorithm HashAlgorithm = new SHA256Managed();

        public static void SetSaltAndHash(ApplicationUser user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(nameof(password));

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = CryptoRandomNumberUtil.GetBytes(SaltLength, true);

            byte[] hashBytes = ComputeHash(passwordBytes, saltBytes);

            user.PasswordHash = Convert.ToBase64String(hashBytes);
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