using Microsoft.EntityFrameworkCore;
using VoidPass.Data;

namespace VoidPass.Services;

public class LimboService
{
    private readonly VoidPassDbContext _db;
    private readonly PasswordGenerator _generator;
    private readonly PasswordHasher _hasher;

    public LimboService(
        VoidPassDbContext db,
        PasswordGenerator generator,
        PasswordHasher hasher)
    {
        _db = db;
        _generator = generator;
        _hasher = hasher;
    }

    public async Task<string> GerarSenhaUnicaAsync(
        int tamanho,
        CancellationToken cancellationToken = default)
    {
        while (true)
        {
            string senha = _generator.Gerar(tamanho);
            string hash = _hasher.Hash(senha);

            bool jaExiste = await _db.UsedPasswords
                .AnyAsync(x => x.Hash == hash, cancellationToken);

            if (jaExiste) continue;

            var registro = new Models.UsedPassword
            {
                Hash = hash,
                CreatedAt = DateTime.UtcNow
            };

            _db.UsedPasswords.Add(registro);

            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return senha;
            }
            catch (DbUpdateException)
            {
                _db.Entry(registro).State = EntityState.Detached;
            }
        }
    }
}