using Microsoft.EntityFrameworkCore;
using Xunit;
using VoidPass.Data;
using VoidPass.Services;

namespace VOID_v2.Tests;

public class LimboServiceTests
{
    [Fact]
    public async Task DeveGerarDuasSenhasUnicasSimultaneamente()
    {
        var options = new DbContextOptionsBuilder<VoidPassDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=voidpass;Username=postgres;Password=d6L1!Y(Jj0!Lq)r6")
            .Options;

        await using var db1 = new VoidPassDbContext(options);
        await using var db2 = new VoidPassDbContext(options);

        var gerador1 = new PasswordGenerator();
        var gerador2 = new PasswordGenerator();

        var hasher1 = new PasswordHasher();
        var hasher2 = new PasswordHasher();

        var limbo1 = new LimboService(
            db1,
            gerador1,
            hasher1);

        var limbo2 = new LimboService(
            db2,
            gerador2,
            hasher2);

        var tarefa1 = limbo1.GerarSenhaUnicaAsync(12);
        var tarefa2 = limbo2.GerarSenhaUnicaAsync(12);

        var resultados = await Task.WhenAll(
            tarefa1,
            tarefa2);

        Assert.Equal(2, resultados.Length);
        Assert.NotEqual(resultados[0], resultados[1]);
    }
}