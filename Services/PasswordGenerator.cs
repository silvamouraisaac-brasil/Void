using System.Security.Cryptography;

namespace VoidPass.Services;

public class PasswordGenerator
{
    private const string Minusculas =
        "abcdefghijklmnopqrstuvwxyz";

    private const string Maiusculas =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private const string Numeros =
        "0123456789";

    private const string Simbolos =
        "!@#$%¨&*()";

    public string Gerar(int tamanho)
    {
        if (tamanho < 12 || tamanho > 16)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tamanho),
                "A senha deve ter entre 12 e 16 caracteres.");
        }

        string todosCaracteres =
            Minusculas +
            Maiusculas +
            Numeros +
            Simbolos;

        var senhaLista = new List<char>
        {
            Minusculas[
                RandomNumberGenerator.GetInt32(
                    0,
                    Minusculas.Length)],

            Maiusculas[
                RandomNumberGenerator.GetInt32(
                    0,
                    Maiusculas.Length)],

            Numeros[
                RandomNumberGenerator.GetInt32(
                    0,
                    Numeros.Length)],

            Simbolos[
                RandomNumberGenerator.GetInt32(
                    0,
                    Simbolos.Length)]
        };

        while (senhaLista.Count < tamanho)
        {
            int indiceAleatorio =
                RandomNumberGenerator.GetInt32(
                    0,
                    todosCaracteres.Length);

            senhaLista.Add(
                todosCaracteres[indiceAleatorio]);
        }

        Embaralhar(senhaLista);

        return new string(senhaLista.ToArray());
    }

    private static void Embaralhar(List<char> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j =
                RandomNumberGenerator.GetInt32(i + 1);

            (lista[i], lista[j]) =
                (lista[j], lista[i]);
        }
    }
}