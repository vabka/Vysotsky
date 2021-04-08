using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vysotsky.Service
{
    public class SecureHasher
    {
        private readonly byte[] _key;

        public SecureHasher(byte[] key)
        {
            _key = key;
        }

        public byte[] Hash(string source)
        {
            using var alg = new HMACSHA256(_key);
            return alg.ComputeHash(Encoding.UTF8.GetBytes(source));
        }
    }

    public static class SecureHasherExtensions
    {
        public static IServiceCollection AddSecureHasher(this IServiceCollection serviceCollection) =>
            serviceCollection.AddSingleton(s =>
            {
                var configuration = s.GetRequiredService<IConfiguration>();
                var salt = configuration["SALT"];
                return new SecureHasher(Convert.FromBase64String(salt));
            });
    }
}
