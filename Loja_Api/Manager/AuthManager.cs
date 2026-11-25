using Loja_Api.Interfaces;
using Loja_Api.Model;
using Loja_Api.Model.Auth;
using Loja_Api.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Loja_Api.Manager
{
    public class AuthManager : IAuthManager
    {
        private readonly IRepositoryBase _repository;
        private readonly JwtTokenService _tokens;

        public AuthManager(IRepositoryBase repository, JwtTokenService tokens)
        {
            _repository = repository;
            _tokens = tokens;
        }

        public Task<IEnumerable<Usuario>> GetAll() => _repository.GetAsync<Usuario>();

        public async Task<Usuario> Post(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);
            if (usuario._id == Guid.Empty)
                usuario._id = Guid.NewGuid();
            if (!SenhaHasher.EhHash(usuario._senha))
                usuario._senha = SenhaHasher.Hash(usuario._senha);
            return await _repository.AddAsync(usuario);
        }

        public async Task<Usuario> Update(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);
            if (!SenhaHasher.EhHash(usuario._senha))
                usuario._senha = SenhaHasher.Hash(usuario._senha);
            return await _repository.Putasync(usuario);
        }

        public Task<bool> Delete(Guid id) => _repository.DeleteAsync<Usuario>(id);

        public async Task<UsuarioConta> Cadastrar(CadastroUsuario cadastro)
        {
            ArgumentNullException.ThrowIfNull(cadastro);
            var email = cadastro.Email.Trim().ToLowerInvariant();
            var nome = cadastro.Nome.Trim();
            if (nome.Length < 2 || !email.Contains('@') || cadastro.Senha.Length < 8)
                throw new ArgumentException("Informe nome, e-mail e senha válidos (mínimo de 8 caracteres).");

            var existente = await _repository.GetAsync<Usuario>(new Dictionary<string, string> { ["_email"] = email });
            if (existente.Any())
                throw new InvalidOperationException("Já existe uma conta cadastrada com este e-mail.");

            var cpf = SomenteDigitos(cadastro.Cpf);
            var telefone = SomenteDigitos(cadastro.Telefone);
            if (cpf.Length != 11 || telefone.Length is < 10 or > 13)
                throw new ArgumentException("CPF ou telefone inválido.");

            var usuario = new Usuario(nome, email, cadastro.Senha, cpf, telefone);
            var criado = await Post(usuario);
            return new UsuarioConta { Nome = criado._nome, Email = criado._email };
        }

        public async Task<LoginResposta?> Login(LoginUsuario credenciais)
        {
            ArgumentNullException.ThrowIfNull(credenciais);
            var email = credenciais.Email.Trim().ToLowerInvariant();
            var usuario = (await _repository.GetAsync<Usuario>(new Dictionary<string, string> { ["_email"] = email }))
                .SingleOrDefault();
            if (usuario is null || !await ValidarSenha(usuario, credenciais.Senha))
                return null;

            return _tokens.CriarToken(usuario);
        }

        private async Task<bool> ValidarSenha(Usuario usuario, string senha)
        {
            if (SenhaHasher.EhHash(usuario._senha))
            {
                if (!SenhaHasher.Verificar(senha, usuario._senha, out var precisaAtualizar))
                    return false;
                if (precisaAtualizar)
                {
                    usuario._senha = SenhaHasher.Hash(senha);
                    await _repository.Putasync(usuario);
                }
                return true;
            }

            // Migra uma senha legada em texto puro no primeiro login bem-sucedido.
            var informada = Encoding.UTF8.GetBytes(senha);
            var legada = Encoding.UTF8.GetBytes(usuario._senha);
            if (informada.Length != legada.Length || !CryptographicOperations.FixedTimeEquals(informada, legada))
                return false;

            usuario._senha = SenhaHasher.Hash(senha);
            await _repository.Putasync(usuario);
            return true;
        }

        private static string SomenteDigitos(string valor) => Regex.Replace(valor ?? string.Empty, "\\D", string.Empty);
    }
}
