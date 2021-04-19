using System.Security.Cryptography;
using System.Text;
using Vysotsky.Service.Interfaces;

namespace Vysotsky.Service.Impl
{
    public class SecureHasher : IStringHasher
    {
        private readonly SecureHasherOptions options;

        public SecureHasher(SecureHasherOptions options) => this.options = options;

        public byte[] Hash(string source)
        {
            var key = Encoding.UTF8.GetBytes(options.Salt);
            using var alg = new HMACSHA512(key);
            return alg.ComputeHash(Encoding.UTF8.GetBytes(source));
        }
    }
}
