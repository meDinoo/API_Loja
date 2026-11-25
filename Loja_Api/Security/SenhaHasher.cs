using System.Security.Cryptography;

namespace Loja_Api.Security
{
    public static class SenhaHasher
    {
        private const int Iteracoes = 210_000;
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const string Prefixo = "pbkdf2-sha256";

        public static string Hash(string senha)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(senha);
            var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
            return $"{Prefixo}${Iteracoes}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool EhHash(string valor) =>
            !string.IsNullOrWhiteSpace(valor) && valor.StartsWith(Prefixo + "$", StringComparison.Ordinal);

        public static bool Verificar(string senha, string valorArmazenado, out bool precisaAtualizar)
        {
            precisaAtualizar = false;
            if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(valorArmazenado))
                return false;

            var partes = valorArmazenado.Split('$');
            if (partes.Length != 4 || partes[0] != Prefixo ||
                !int.TryParse(partes[1], out var iteracoes) || iteracoes < 100_000)
                return false;

            try
            {
                var salt = Convert.FromBase64String(partes[2]);
                var hashEsperado = Convert.FromBase64String(partes[3]);
                var hashInformado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, hashEsperado.Length);
                precisaAtualizar = iteracoes < Iteracoes;
                return CryptographicOperations.FixedTimeEquals(hashInformado, hashEsperado);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
