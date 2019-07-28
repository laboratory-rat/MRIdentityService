using System;
using System.Security.Cryptography;

namespace Tools
{
    public static class KeyGenerator
    {
        private static Random _randCache;
        private static Random _rand
        {
            get
            {
                if(_randCache == null)
                {
                    _randCache = new Random();
                }
                return _rand;
            }
        }

        public static string GenerateAccessKey(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bit_count = (length * 6);
                var byte_count = ((bit_count + 7) / 8); // rounded up
                var bytes = new byte[byte_count];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("/", "_");
            }
        }
    }
}
