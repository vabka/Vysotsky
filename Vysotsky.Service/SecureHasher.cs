using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vysotsky.Service
{
    public interface IStringHasher
    {
        byte[] Hash(string source);
    }

    public class SecureHasher : IStringHasher
    {
        private readonly byte[] _key;

        public SecureHasher(byte[] key)
        {
            _key = key;
        }

        public byte[] Hash(string source)
        {
            using var alg = new HMACSHA512(_key);
            return alg.ComputeHash(Encoding.UTF8.GetBytes(source));
        }
    }

    public static class SecureHasherExtensions
    {
        public static IServiceCollection AddSecureHasher(this IServiceCollection serviceCollection) =>
            serviceCollection.AddSingleton<IStringHasher>(s =>
            {
                var configuration = s.GetRequiredService<IConfiguration>();
                var salt = configuration["SALT"];
                return new SecureHasher(Convert.FromBase64String(salt));
            });
    }
}