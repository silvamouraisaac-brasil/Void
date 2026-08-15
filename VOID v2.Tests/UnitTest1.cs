using Microsoft.EntityFrameworkCore;
using Xunit;
using VoidPass.Data;
using VoidPass.Services;

namespace VOID_v2.Tests;

public class PasswordGeneratorTests
{
    [Fact]
    public void DeveGerarUmaSenha()
    {
        var gerador = new PasswordGenerator();

        var senha = gerador.Gerar(12);

        Assert.False(string.IsNullOrWhiteSpace(senha));
    }

    [Fact]
    public void DeveGerarSenhaComTamanhoCorreto()
    {
        var gerador = new PasswordGenerator();

        var senha = gerador.Gerar(12);

        Assert.Equal(12, senha.Length);
    }

    [Fact]
    public void DeveConterTodosOsTiposDeCaracteres()
    {
        var gerador = new PasswordGenerator();

        var senha = gerador.Gerar(12);

        Assert.Contains(senha, char.IsLower);
        Assert.Contains(senha, char.IsUpper);
        Assert.Contains(senha, char.IsDigit);

        Assert.Contains(
            senha,
            caractere => "!@#$%¨&*()".Contains(caractere));
    }

    [Fact]
    public void DeveRejeitarSenhaMenorQue12Caracteres()
    {
        var gerador = new PasswordGenerator();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => gerador.Gerar(11));
    }

    [Fact]
    public void DeveRejeitarSenhaMaiorQue16Caracteres()
    {
        var gerador = new PasswordGenerator();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => gerador.Gerar(17));
    }

    [Fact]
    public void DeveGerarSenhasDiferentes()
    {
        var gerador = new PasswordGenerator();

        var senha1 = gerador.Gerar(16);
        var senha2 = gerador.Gerar(16);

        Assert.NotEqual(senha1, senha2);
    }

    [Fact]
    public void DeveAceitarTamanhoMinimo()
    {
        var gerador = new PasswordGenerator();

        var senha = gerador.Gerar(12);

        Assert.Equal(12, senha.Length);
    }

    [Fact]
    public void DeveAceitarTamanhoMaximo()
    {
        var gerador = new PasswordGenerator();

        var senha = gerador.Gerar(16);

        Assert.Equal(16, senha.Length);
    }

    [Fact]
    public void DeveGerarMesmoHashParaMesmaSenha()
    {
        var hasher = new PasswordHasher();

        var hash1 = hasher.Hash("MinhaSenha123!");
        var hash2 = hasher.Hash("MinhaSenha123!");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void DeveGerarHashesDiferentesParaSenhasDiferentes()
    {
        var hasher = new PasswordHasher();

        var hash1 = hasher.Hash("MinhaSenha123!");
        var hash2 = hasher.Hash("MinhaSenha456!");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashNaoDeveSerIgualASenhaOriginal()
    {
        var hasher = new PasswordHasher();

        var senha = "MinhaSenha123!";
        var hash = hasher.Hash(senha);

        Assert.NotEqual(senha, hash);
    }

    [Fact]
    public async Task LimboDeveGerarSenhaUnica()
    {
        var gerador = new PasswordGenerator();
        var hasher = new PasswordHasher();

        var connectionString =
            Environment.GetEnvironmentVariable("VOID_TEST_CONNECTION");

        Assert.False(
            string.IsNullOrWhiteSpace(connectionString),
            "A variável de ambiente VOID_TEST_CONNECTION não está configurada.");

        var options = new DbContextOptionsBuilder<VoidPassDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var db = new VoidPassDbContext(options);

        var limbo = new LimboService(
            db,
            gerador,
            hasher);

        var senha1 = await limbo.GerarSenhaUnicaAsync(12);
        var senha2 = await limbo.GerarSenhaUnicaAsync(12);

        Assert.NotEqual(senha1, senha2);

        var hash1 = hasher.Hash(senha1);
        var hash2 = hasher.Hash(senha2);

        Assert.NotEqual(hash1, hash2);

        var quantidadeHash1 = await db.UsedPasswords
            .CountAsync(x => x.Hash == hash1);

        var quantidadeHash2 = await db.UsedPasswords
            .CountAsync(x => x.Hash == hash2);

        Assert.Equal(1, quantidadeHash1);
        Assert.Equal(1, quantidadeHash2);
    }
}