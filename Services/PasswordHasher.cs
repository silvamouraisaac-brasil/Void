using System.Security.Cryptography;
using System.Text;

namespace VoidPass.Services;

public class PasswordHasher
{
    public string Hash(string senha)
    {
        ArgumentNullException.ThrowIfNull(senha);

        byte[] bytes = Encoding.UTF8.GetBytes(senha);

        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}