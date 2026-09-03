using System.Security.Cryptography;
using System.Text;

namespace Event_parking.Helpers
{
    public static class TokenHelper
    {
        public static string CreateToken()
        {
            byte[] tokenBytes =
                RandomNumberGenerator.GetBytes(32);

            return Convert.ToHexString(tokenBytes);
        }

        public static string HashToken(string token)
        {
            byte[] tokenBytes =
                Encoding.UTF8.GetBytes(token);

            byte[] hashBytes =
                SHA256.HashData(tokenBytes);

            return Convert.ToHexString(hashBytes);
        }
    }
}